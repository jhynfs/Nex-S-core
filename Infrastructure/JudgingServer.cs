using MongoDB.Driver;
using MongoDB.Bson;
using NexScore.Models;
using NexScore.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NexScore.Infrastructure
{
    public class JudgingServer : IDisposable
    {
        private readonly HttpListener _listener = new();
        private readonly IMongoDatabase _db;
        private readonly ScoringService _scoring;
        private readonly string _staticRoot;
        private CancellationTokenSource _cts = new();
        private bool _running;

        private static readonly ConcurrentDictionary<string, LiveState> _liveStates =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, DateTime> _judgeHeartbeats =
            new(StringComparer.OrdinalIgnoreCase);

        public JudgingServer(IMongoDatabase db, string staticRoot)
        {
            _db = db;
            _scoring = new ScoringService(db);
            _staticRoot = staticRoot;
        }

        public void Start(int port = 5100)
        {
            if (_running) return;
            EnsureIndexes();
            string prefix = $"http://+:{port}/";
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            _running = true;
            _ = Task.Run(() => AcceptLoop(_cts.Token));
        }

        public void Stop()
        {
            if (!_running) return;
            _cts.Cancel();
            try { _listener.Stop(); } catch { }
            _running = false;
        }

        private async Task AcceptLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var ctx = await _listener.GetContextAsync();
                    _ = Task.Run(() => Handle(ctx));
                }
                catch when (ct.IsCancellationRequested) { return; }
                catch { }
            }
        }

        private void EnsureIndexes()
        {
            try
            {
                var scores = _db.GetCollection<ScoreEntry>("Scores");
                var keys = Builders<ScoreEntry>.IndexKeys
                    .Ascending(e => e.EventId)
                    .Ascending(e => e.ContestantId)
                    .Ascending(e => e.JudgeId)
                    .Ascending(e => e.CriteriaId);

                scores.Indexes.CreateOne(new CreateIndexModel<ScoreEntry>(
                    keys,
                    new CreateIndexOptions
                    {
                        Unique = true,
                        Name = "uniq_score_event_contestant_judge_criteria"
                    }));
            }
            catch { }
        }

        private static void SetCors(HttpListenerResponse res)
        {
            res.Headers["Access-Control-Allow-Origin"] = "*";
            res.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
            res.Headers["Access-Control-Allow-Headers"] = "Content-Type, X-Requested-With";
            res.Headers["Access-Control-Max-Age"] = "86400";
        }

        private async Task Handle(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var res = ctx.Response;

            try
            {
                SetCors(res);

               
                if (req.HttpMethod == "OPTIONS")
                {
                    res.StatusCode = 204;
                    res.OutputStream.Close();
                    return;
                }

                string path = req.Url?.AbsolutePath ?? "/";

                if (path == "/")
                {
                    Redirect(res, "/JudgeClient/index.html");
                    return;
                }

                if (path.StartsWith("/JudgeClient/", StringComparison.OrdinalIgnoreCase))
                {
                    await ServeStaticAsync(res, path.Substring(1));
                    return;
                }

                // EVENT META (EventModel fields)
                if (path.Equals("/api/event", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId)) { BadRequest(res, "eventId required"); return; }

                    var evt = await _db.GetCollection<EventModel>("Events")
                                       .Find(e => e.Id == eventId).FirstOrDefaultAsync();
                    if (evt == null) { NotFound(res); return; }

                    await JsonAsync(res, new
                    {
                        evt.Id,
                        evt.EventId,
                        evt.EventName,
                        evt.Description,
                        evt.Venue,
                        evt.Organizers,
                        evt.EventDate,
                        evt.BannerPath
                    });
                    return;
                }

                // STRUCTURE
                if (path.Equals("/api/structure", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    var s = await _db.GetCollection<EventStructureModel>("EventStructures")
                                     .Find(x => x.EventId == eventId)
                                     .FirstOrDefaultAsync();
                    if (s == null) { NotFound(res); return; }
                    await JsonAsync(res, s);
                    return;
                }

                // CONTESTANTS (active by default)
                if (path.Equals("/api/contestants", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    bool showAll = string.Equals(req.QueryString["showAll"], "true", StringComparison.OrdinalIgnoreCase);

                    var col = _db.GetCollection<ContestantModel>("Contestants");

                    var baseFilter = Builders<ContestantModel>.Filter.Eq(c => c.EventId, eventId);
                    if (!showAll)
                    {
                        var activeOrMissing = Builders<ContestantModel>.Filter.Or(
                            Builders<ContestantModel>.Filter.Eq(c => c.IsActive, true),
                            Builders<ContestantModel>.Filter.Exists(nameof(ContestantModel.IsActive), false)
                        );
                        baseFilter = Builders<ContestantModel>.Filter.And(baseFilter, activeOrMissing);
                    }

                    var list = await col.Find(baseFilter).SortBy(c => c.Number).ToListAsync();

                    await JsonAsync(res, list.Select(c => new
                    {
                        c.Id,
                        c.Number,
                        c.FullName,
                        c.Representing,
                        c.Gender,
                        c.PhotoPath,
                        c.Age,
                        c.IsActive
                    }));
                    return;
                }

                // UPDATE ACTIVE FLAGS
                if (path.Equals("/api/contestants/update-active", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "POST")
                {
                    try
                    {
                        using var reader = new System.IO.StreamReader(req.InputStream);
                        var body = await reader.ReadToEndAsync();
                        var doc = System.Text.Json.JsonDocument.Parse(body);

                        if (!doc.RootElement.TryGetProperty("eventId", out var evtProp))
                        {
                            BadRequest(res, "Missing eventId");
                            return;
                        }
                        string eventId = evtProp.GetString() ?? "";

                        if (!doc.RootElement.TryGetProperty("updates", out var updatesProp) || updatesProp.ValueKind != System.Text.Json.JsonValueKind.Array)
                        {
                            BadRequest(res, "Missing updates array");
                            return;
                        }

                        var contestantCol = _db.GetCollection<ContestantModel>("Contestants");
                        int changed = 0;

                        foreach (var u in updatesProp.EnumerateArray())
                        {
                            if (!u.TryGetProperty("contestantId", out var cidProp)) continue;
                            if (!u.TryGetProperty("isActive", out var activeProp)) continue;
                            string contestantId = cidProp.GetString() ?? "";
                            bool isActive = activeProp.GetBoolean();

                            var filter = Builders<ContestantModel>.Filter.And(
                                Builders<ContestantModel>.Filter.Eq(c => c.Id, contestantId),
                                Builders<ContestantModel>.Filter.Eq(c => c.EventId, eventId)
                            );

                            var update = Builders<ContestantModel>.Update
                                .Set(c => c.IsActive, isActive)
                                .Set(c => c.UpdatedAt, DateTime.UtcNow);

                            var result = await contestantCol.UpdateOneAsync(filter, update);
                            if (result.ModifiedCount > 0) changed++;
                        }

                        await JsonAsync(res, new { status = "ok", changed });
                        return;
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 500;
                        await JsonAsync(res, new { error = "update-active failed", message = ex.Message });
                        return;
                    }
                }

                // JUDGES
                if (path.Equals("/api/judges", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    var list = await _db.GetCollection<JudgeModel>("Judges")
                                        .Find(j => j.EventId == eventId)
                                        .SortBy(j => j.Number)
                                        .ToListAsync();
                    await JsonAsync(res, list.Select(j => new
                    {
                        j.Id,
                        j.JudgeId,
                        j.Name,
                        j.Number,
                        j.Title
                    }));
                    return;
                }

                // JUDGE LOGIN (optional PIN)
                if (path.Equals("/api/judge-login", StringComparison.OrdinalIgnoreCase)
                    && req.HttpMethod == "POST")
                {
                    using var sr = new StreamReader(req.InputStream, req.ContentEncoding);
                    var raw = await sr.ReadToEndAsync();
                    var dto = JsonSerializer.Deserialize<JudgeLoginDto>(raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto == null || string.IsNullOrWhiteSpace(dto.EventId) || string.IsNullOrWhiteSpace(dto.JudgeId))
                    {
                        BadRequest(res, "eventId and judgeId required");
                        return;
                    }

                    var judgesCol = _db.GetCollection<BsonDocument>("Judges");

                    BsonDocument? jdoc = null;
                    if (ObjectId.TryParse(dto.JudgeId, out var oid))
                    {
                        jdoc = await judgesCol.Find(Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("eventId", dto.EventId),
                                        Builders<BsonDocument>.Filter.Eq("_id", oid)))
                                .FirstOrDefaultAsync();
                    }
                    if (jdoc == null)
                    {
                        jdoc = await judgesCol.Find(Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("eventId", dto.EventId),
                                        Builders<BsonDocument>.Filter.Eq("judgeId", dto.JudgeId)))
                                .FirstOrDefaultAsync();
                    }

                    if (jdoc == null)
                    {
                        NotFound(res);
                        return;
                    }

                    var pinValue = jdoc.TryGetValue("pin", out var pinB) && pinB.IsString
                        ? pinB.AsString
                        : "";
                    if (!string.IsNullOrEmpty(pinValue) && pinValue != (dto.Pin ?? ""))
                    {
                        res.StatusCode = 401;
                        await WriteUtf8Async(res, "{\"error\":\"invalid pin\"}");
                        return;
                    }

                    string outId =
                        (jdoc.TryGetValue("judgeId", out var jIdVal) && jIdVal.IsString)
                            ? jIdVal.AsString
                            : dto.JudgeId;

                    await JsonAsync(res, new { judgeId = outId });
                    return;
                }

                // LIVE STATE GET
                if (path.Equals("/api/live-state", StringComparison.OrdinalIgnoreCase)
                    && req.HttpMethod == "GET")
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    _liveStates.TryGetValue(eventId, out var state);
                    await JsonAsync(res, state ?? new LiveState
                    {
                        EventId = eventId,
                        UpdatedAt = DateTime.UtcNow
                    });
                    return;
                }

                // LIVE STATE POST
                if (path.Equals("/api/live-state", StringComparison.OrdinalIgnoreCase)
                    && req.HttpMethod == "POST")
                {
                    using var sr = new StreamReader(req.InputStream, req.ContentEncoding);
                    var raw = await sr.ReadToEndAsync();
                    var dto = JsonSerializer.Deserialize<LiveStateDto>(raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto == null || string.IsNullOrWhiteSpace(dto.EventId))
                    {
                        BadRequest(res, "invalid live state data");
                        return;
                    }

                    _liveStates[dto.EventId] = new LiveState
                    {
                        EventId = dto.EventId,
                        PhaseKey = dto.PhaseKey,
                        SegmentKey = dto.SegmentKey,
                        ContestantId = dto.ContestantId,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await JsonAsync(res, new { status = "ok" });
                    return;
                }

                // JUDGE HEARTBEAT
                if (path.Equals("/api/judge-heartbeat", StringComparison.OrdinalIgnoreCase)
                    && req.HttpMethod == "POST")
                {
                    using var sr = new StreamReader(req.InputStream, req.ContentEncoding);
                    var raw = await sr.ReadToEndAsync();
                    var dto = JsonSerializer.Deserialize<JudgeHeartbeatDto>(raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto == null ||
                        string.IsNullOrWhiteSpace(dto.EventId) ||
                        string.IsNullOrWhiteSpace(dto.JudgeId))
                    {
                        BadRequest(res, "invalid heartbeat");
                        return;
                    }

                    string key = dto.EventId + "::" + dto.JudgeId;
                    _judgeHeartbeats[key] = DateTime.UtcNow;
                    await JsonAsync(res, new { status = "ok" });
                    return;
                }

                // ONLINE JUDGES
                if (path.Equals("/api/judges-online", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    DateTime cutoff = DateTime.UtcNow.AddSeconds(-90);
                    var list = _judgeHeartbeats
                        .Where(kv => kv.Key.StartsWith(eventId + "::", StringComparison.OrdinalIgnoreCase))
                        .Select(kv =>
                        {
                            var judgeIdPart = kv.Key.Substring(eventId.Length + 2);
                            return new
                            {
                                judgeId = judgeIdPart,
                                lastSeen = kv.Value,
                                online = kv.Value >= cutoff
                            };
                        })
                        .ToList();
                    await JsonAsync(res, list);
                    return;
                }

                // SCORE (UPSERT)
                if (path.Equals("/api/score", StringComparison.OrdinalIgnoreCase)
                    && req.HttpMethod == "POST")
                {
                    using var sr = new StreamReader(req.InputStream, req.ContentEncoding);
                    var raw = await sr.ReadToEndAsync();
                    var dto = JsonSerializer.Deserialize<ScoreEntryDto>(raw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto == null)
                    {
                        BadRequest(res, "invalid json");
                        return;
                    }

                    var now = DateTime.UtcNow;
                    var scores = _db.GetCollection<ScoreEntry>("Scores");

                    var keyFilter = Builders<ScoreEntry>.Filter.Where(e =>
                        e.EventId == dto.EventId &&
                        e.ContestantId == dto.ContestantId &&
                        e.JudgeId == dto.JudgeId &&
                        e.CriteriaId == (dto.CriteriaId ?? "").ToLowerInvariant());

                    var update = Builders<ScoreEntry>.Update
                        .Set(e => e.PhaseId, dto.PhaseId)
                        .Set(e => e.SegmentId, dto.SegmentId)
                        .Set(e => e.CriteriaId, (dto.CriteriaId ?? "").ToLowerInvariant())
                        .Set(e => e.RawValue, dto.RawValue)
                        .Set(e => e.MaxValue, dto.MaxValue <= 0 ? 100 : dto.MaxValue)
                        .Set(e => e.UpdatedAt, now)
                        .SetOnInsert(e => e.EventId, dto.EventId)
                        .SetOnInsert(e => e.ContestantId, dto.ContestantId)
                        .SetOnInsert(e => e.JudgeId, dto.JudgeId)
                        .SetOnInsert(e => e.CreatedAt, now)
                        .SetOnInsert(e => e.IsFinalized, false);

                    await scores.UpdateOneAsync(keyFilter, update, new UpdateOptions { IsUpsert = true });

                    await _scoring.RecomputeContestantAsync(dto.EventId, dto.ContestantId);

                    await JsonAsync(res, new { status = "ok" });
                    return;
                }

                // JUDGE PROGRESS
                if (path.Equals("/api/judge-progress", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "GET")
                {
                    try
                    {
                        var eventIdQ = req.QueryString["eventId"];
                        var judgeIdQ = req.QueryString["judgeId"];
                        if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(judgeIdQ))
                        {
                            BadRequest(res, "eventId and judgeId are required");
                            return;
                        }

                        // Pick a scores collection that exists
                        async Task<string> PickScoresCollectionAsync()
                        {
                            var preferred = new[] { "Scores", "ScoreEntries", "JudgeScores" };
                            var existing = await (await _db.ListCollectionNamesAsync()).ToListAsync();
                            foreach (var p in preferred)
                                if (existing.Contains(p)) return p;
                            // Fallback to "Scores" even if not listed (driver will allow creating it on the fly)
                            return "Scores";
                        }

                        var colName = await PickScoresCollectionAsync();
                        var col = _db.GetCollection<BsonDocument>(colName);

                        // Filter (expect EventId and JudgeId stored as strings; sample shows string fields)
                        var filter = Builders<BsonDocument>.Filter.And(
                            Builders<BsonDocument>.Filter.Eq("EventId", eventIdQ),
                            Builders<BsonDocument>.Filter.Eq("JudgeId", judgeIdQ)
                        );

                        // Project only fields we need; missing fields are handled safely below
                        var proj = Builders<BsonDocument>.Projection
                            .Include("ContestantId")
                            .Include("RawValue")
                            .Include("MaxValue");

                        var docs = await col.Find(filter).Project(proj).ToListAsync();

                        // Group by contestant and compute average normalized score
                        var totals = docs
                            .GroupBy(d => d.TryGetValue("ContestantId", out var v) ? v.AsString : "")
                            .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                            .Select(g =>
                            {
                                double sum = 0.0;
                                int n = 0;
                                foreach (var d in g)
                                {
                                    double raw = d.Contains("RawValue") && d["RawValue"].IsNumeric ? d["RawValue"].ToDouble() : 0.0;
                                    double max = d.Contains("MaxValue") && d["MaxValue"].IsNumeric ? d["MaxValue"].ToDouble() : 100.0;
                                    if (max <= 0) max = 100.0;
                                    var frac = raw / max;
                                    if (double.IsNaN(frac) || double.IsInfinity(frac)) continue;
                                    if (frac < 0) frac = 0;
                                    if (frac > 1) frac = 1;
                                    sum += frac;
                                    n++;
                                }
                                double avg = n > 0 ? sum / n : 0.0;
                                return new { contestantId = g.Key, totalFraction = avg };
                            })
                            .OrderByDescending(x => x.totalFraction)
                            .ToList();

                        await JsonAsync(res, totals);
                        return;
                    }
                    catch (Exception ex)
                    {
                        // Return detailed error so you can see the cause in the browser
                        res.StatusCode = 500;
                        await JsonAsync(res, new { error = "judge-progress failed", message = ex.Message, stack = ex.StackTrace });
                        return;
                    }
                }

                // Criteria-level rows for a judge + contestant (used by "View Criteria")
                if (path.Equals("/api/judge-scores", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "GET")
                {
                    try
                    {
                        var eventIdQ = req.QueryString["eventId"];
                        var judgeIdQ = req.QueryString["judgeId"];
                        var contestantIdQ = req.QueryString["contestantId"];
                        if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(judgeIdQ) || string.IsNullOrWhiteSpace(contestantIdQ))
                        {
                            BadRequest(res, "eventId, judgeId, contestantId required");
                            return;
                        }

                        async Task<string> PickScoresCollectionAsync()
                        {
                            var preferred = new[] { "Scores", "ScoreEntries", "JudgeScores" };
                            var existing = await (await _db.ListCollectionNamesAsync()).ToListAsync();
                            foreach (var p in preferred)
                                if (existing.Contains(p)) return p;
                            return "Scores";
                        }

                        var colName = await PickScoresCollectionAsync();
                        var col = _db.GetCollection<BsonDocument>(colName);

                        var filter = Builders<BsonDocument>.Filter.And(
                            Builders<BsonDocument>.Filter.Eq("EventId", eventIdQ),
                            Builders<BsonDocument>.Filter.Eq("JudgeId", judgeIdQ),
                            Builders<BsonDocument>.Filter.Eq("ContestantId", contestantIdQ)
                        );

                        var proj = Builders<BsonDocument>.Projection
                            .Include("PhaseId")
                            .Include("SegmentId")
                            .Include("CriteriaId")
                            .Include("RawValue")
                            .Include("MaxValue")
                            .Include("CreatedAt")
                            .Include("UpdatedAt");

                        var list = await col.Find(filter).Project(proj).Sort(Builders<BsonDocument>.Sort.Ascending("CreatedAt")).ToListAsync();

                        // Return as-is (BsonDocument converts naturally to JSON)
                        await JsonAsync(res, list);
                        return;
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 500;
                        await JsonAsync(res, new { error = "judge-scores failed", message = ex.Message, stack = ex.StackTrace });
                        return;
                    }
                }

                if (path.Equals("/api/judge-scores", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "GET")
                {
                    try
                    {
                        var eventIdQ = req.QueryString["eventId"];
                        var judgeIdQ = req.QueryString["judgeId"];
                        var contestantIdQ = req.QueryString["contestantId"];
                        if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(judgeIdQ) || string.IsNullOrWhiteSpace(contestantIdQ))
                        {
                            BadRequest(res, "eventId, judgeId, contestantId required");
                            return;
                        }

                        var scoresCol = _db.GetCollection<ScoreEntry>("Scores"); // adjust if needed
                        var list = await scoresCol
                            .Find(s => s.EventId == eventIdQ && s.JudgeId == judgeIdQ && s.ContestantId == contestantIdQ)
                            .SortBy(s => s.CreatedAt)
                            .ToListAsync();

                        await JsonAsync(res, list);
                        return;
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 500;
                        await JsonAsync(res, new { error = "judge-scores failed", message = ex.Message });
                        return;
                    }
                }

                // RESTORE SCORES FOR ONE CONTESTANT
                if (path.Equals("/api/judge-scores", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    string judgeIdQ = req.QueryString["judgeId"];
                    string contestantId = req.QueryString["contestantId"];

                    if (string.IsNullOrWhiteSpace(eventId) ||
                        string.IsNullOrWhiteSpace(judgeIdQ) ||
                        string.IsNullOrWhiteSpace(contestantId))
                    {
                        BadRequest(res, "eventId & judgeId & contestantId required");
                        return;
                    }

                    var scoresCol = _db.GetCollection<ScoreEntry>("Scores");
                    var list = await scoresCol.Find(s =>
                        s.EventId == eventId &&
                        s.JudgeId == judgeIdQ &&
                        s.ContestantId == contestantId).ToListAsync();

                    await JsonAsync(res, list.Select(s => new
                    {
                        s.CriteriaId,
                        s.RawValue,
                        s.MaxValue,
                        s.PhaseId,
                        s.SegmentId
                    }));
                    return;
                }

                if (path.Equals("/api/judge-all-scores", StringComparison.OrdinalIgnoreCase))
                {
                    string eventIdQ = req.QueryString["eventId"];
                    string judgeIdQ = req.QueryString["judgeId"];
                    if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(judgeIdQ))
                    {
                        BadRequest(res, "eventId & judgeId required");
                        return;
                    }
                    var scoresCol = _db.GetCollection<ScoreEntry>("Scores");
                    var list = await scoresCol.Find(s => s.EventId == eventIdQ && s.JudgeId == judgeIdQ).ToListAsync();
                    await JsonAsync(res, list.Select(s => new {
                        s.ContestantId,
                        s.CriteriaId,
                        s.RawValue,
                        s.MaxValue,
                        s.PhaseId,
                        s.SegmentId
                    }));
                    return;
                }

                // Contestant photo streaming endpoint
                if (path.Equals("/api/contestant-photo", StringComparison.OrdinalIgnoreCase))
                {
                    string eventIdQ = req.QueryString["eventId"];
                    string contestantIdQ = req.QueryString["contestantId"];
                    if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(contestantIdQ))
                    {
                        BadRequest(res, "eventId & contestantId required");
                        return;
                    }
                    var contestantsCol = _db.GetCollection<ContestantModel>("Contestants");
                    var c = await contestantsCol.Find(x => x.EventId == eventIdQ && x.Id == contestantIdQ).FirstOrDefaultAsync();
                    if (c == null || string.IsNullOrWhiteSpace(c.PhotoPath) || !System.IO.File.Exists(c.PhotoPath))
                    {
                        // Return 1x1 transparent PNG when missing
                        byte[] placeholder = Convert.FromBase64String(
                            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR4nGNgYGBgAAAABQABDQottAAAAABJRU5ErkJggg==");
                        res.ContentType = "image/png";
                        res.ContentLength64 = placeholder.Length;
                        await res.OutputStream.WriteAsync(placeholder, 0, placeholder.Length);
                        res.OutputStream.Close();
                        return;
                    }
                    try
                    {
                        var ext = System.IO.Path.GetExtension(c.PhotoPath).ToLowerInvariant();
                        res.ContentType = ext switch
                        {
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            ".gif" => "image/gif",
                            _ => "application/octet-stream"
                        };
                        byte[] imgBytes = await System.IO.File.ReadAllBytesAsync(c.PhotoPath);
                        res.ContentLength64 = imgBytes.Length;
                        await res.OutputStream.WriteAsync(imgBytes, 0, imgBytes.Length);
                        res.OutputStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Error(res, "photo read failed: " + ex.Message);
                    }
                    return;
                }
                if (path.Equals("/api/judge-progress", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "GET")
                {
                    try
                    {
                        string eventIdQ = req.QueryString["eventId"];
                        string judgeIdQ = req.QueryString["judgeId"];
                        if (string.IsNullOrWhiteSpace(eventIdQ) || string.IsNullOrWhiteSpace(judgeIdQ))
                        {
                            BadRequest(res, "eventId and judgeId are required");
                            return;
                        }

                        var scoresCol = _db.GetCollection<ScoreEntry>("Scores"); // adjust collection name if different
                        var structCol = _db.GetCollection<EventStructureModel>("EventStructures");

                        // Fetch this judge's scores for the event
                        var judgeScores = await scoresCol
                            .Find(s => s.EventId == eventIdQ && s.JudgeId == judgeIdQ)
                            .ToListAsync();

                        // Early exit: no scores yet
                        if (judgeScores.Count == 0)
                        {
                            await JsonAsync(res, Array.Empty<object>());
                            return;
                        }

                        // Load structure (optional; we’ll fallback if missing)
                        var structure = await structCol.Find(s => s.EventId == eventIdQ).FirstOrDefaultAsync();

                        // Build quick lookup maps from structure (if available)
                        // Phase key style expected: "P{seq}" or "IND-{sanitizedName}" (same as your client)
                        var phaseMap = new Dictionary<string, PhaseModel>(StringComparer.OrdinalIgnoreCase);
                        var segMap = new Dictionary<(string phaseKey, int segSeq), SegmentModel>();
                        var critWeightMap = new Dictionary<(string phaseKey, int segSeq, string critNameLower), decimal>();

                        if (structure?.Phases != null)
                        {
                            string Sanitize(string? name) =>
                                string.IsNullOrWhiteSpace(name) ? "phase" : name.Trim().Replace(" ", "_").ToLowerInvariant();

                            foreach (var p in structure.Phases)
                            {
                                string pKey = p.Sequence.HasValue ? ("P" + p.Sequence.Value) : ("IND-" + Sanitize(p.Name));
                                phaseMap[pKey] = p;

                                if (p.Segments != null)
                                {
                                    foreach (var sgm in p.Segments)
                                    {
                                        segMap[(pKey, sgm.Sequence)] = sgm;

                                        if (sgm.Criteria != null)
                                        {
                                            foreach (var cr in sgm.Criteria)
                                            {
                                                var key = (pKey, sgm.Sequence, (cr.Name ?? "").Trim().ToLowerInvariant());
                                                critWeightMap[key] = cr.Weight;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Helper: parse segment integer from "S{n}"
                        static int ParseSegSeq(string? segId)
                        {
                            if (string.IsNullOrWhiteSpace(segId)) return -1;
                            if (segId.StartsWith("S", StringComparison.OrdinalIgnoreCase))
                            {
                                if (int.TryParse(segId.Substring(1), out var n)) return n;
                            }
                            // fallback if caller sent just a number or something else
                            if (int.TryParse(segId, out var direct)) return direct;
                            return -1;
                        }

                        // Helper: get criteria name (lower) from CriteriaId if needed (format often "P1::1:criteria")
                        static string ExtractCritNameLower(string criteriaId)
                        {
                            if (string.IsNullOrEmpty(criteriaId)) return "";
                            var last = criteriaId.Split(':').LastOrDefault() ?? criteriaId;
                            return last.Trim().ToLowerInvariant();
                        }

                        // Compute totals per contestant
                        var result = judgeScores
                            .GroupBy(s => s.ContestantId)
                            .Select(g =>
                            {
                                double weightedSum = 0.0;
                                double independentSum = 0.0;
                                int countNoStruct = 0;
                                double sumNoStruct = 0.0;

                                foreach (var s in g)
                                {
                                    // Normalize score (0..1)
                                    double max = (s.MaxValue > 0) ? s.MaxValue : 100.0;
                                    double norm = Math.Max(0.0, Math.Min(1.0, s.RawValue / max));

                                    // If no structure, just accumulate to fallback
                                    if (structure?.Phases == null || phaseMap.Count == 0)
                                    {
                                        sumNoStruct += norm;
                                        countNoStruct++;
                                        continue;
                                    }

                                    // With structure, try to weight properly
                                    string pKey = s.PhaseId ?? "";
                                    phaseMap.TryGetValue(pKey, out var phase);

                                    // If unknown phase, fallback accumulate
                                    if (phase == null)
                                    {
                                        sumNoStruct += norm;
                                        countNoStruct++;
                                        continue;
                                    }

                                    bool isIndependent = phase.IsIndependent;

                                    if (isIndependent)
                                    {
                                        // Independent phases: add raw fraction directly (consistent with your Results separation)
                                        independentSum += norm;
                                        continue;
                                    }

                                    // Main (weighted) phases
                                    decimal pW = phase.Weight; // already stored as 0..100 in model
                                    int segSeq = ParseSegSeq(s.SegmentId);
                                    segMap.TryGetValue((pKey, segSeq), out var seg);

                                    decimal sW = seg?.Weight ?? 0m; // 0 if missing
                                    string critNameLower = ExtractCritNameLower(s.CriteriaId ?? "");
                                    critWeightMap.TryGetValue((pKey, segSeq, critNameLower), out var cW);

                                    // Weights are % in DB; convert to fractions
                                    double pF = (double)(pW / 100m);
                                    double sF = (double)(sW / 100m);
                                    double cF = (double)((cW > 0m ? cW : 0m) / 100m);

                                    // If any piece missing, fallback gracefully:
                                    // - If we have phase weight but segment/criteria missing, still use phase weight only.
                                    // - If everything missing, fallback accumulate (unweighted).
                                    double weight =
                                        (pF > 0 && sF > 0 && cF > 0) ? (pF * sF * cF) :
                                        (pF > 0 && sF > 0 && cF == 0) ? (pF * sF) :
                                        (pF > 0 && sF == 0 && cF == 0) ? pF :
                                        0.0;

                                    if (weight > 0.0)
                                    {
                                        weightedSum += norm * weight;
                                    }
                                    else
                                    {
                                        // fallback into "no structure" bucket so it still counts
                                        sumNoStruct += norm;
                                        countNoStruct++;
                                    }
                                }

                                // Combine totals:
                                // - weightedSum is already in fraction (<= 1 if the structure is coherent).
                                // - independentSum is sum of raw fractions from independent phases.
                                // - noStruct fallback: average of normalized scores (so it doesn't blow up with many criteria)
                                double fallbackAvg = (countNoStruct > 0) ? (sumNoStruct / countNoStruct) : 0.0;

                                // Final total fraction:
                                double total = weightedSum + independentSum + fallbackAvg;

                                // Keep within [0, 1.0 + independent], but clamp hard to 1.0 if you prefer.
                                // Here we allow >1 if you have independent phases added on top; you can clamp if desired:
                                if (total < 0) total = 0;
                                // total = Math.Min(total, 1.0); // uncomment to clamp

                                return new
                                {
                                    contestantId = g.Key,
                                    totalFraction = total
                                };
                            })
                            .OrderByDescending(x => x.totalFraction)
                            .ToList();

#if DEBUG
                        // Optional: return as-is
#endif
                        await JsonAsync(res, result);
                        return;
                    }
                    catch (Exception ex)
                    {
                        // Surface the exception to help diagnose instead of a blank 500
                        res.StatusCode = 500;
                        await JsonAsync(res, new { error = "judge-progress failed", message = ex.Message, stack = ex.StackTrace });
                        return;
                    }
                }
                // Event banner streaming endpoint
                if (path.Equals("/api/event-banner", StringComparison.OrdinalIgnoreCase))
                {
                    string eventIdQ = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventIdQ))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    var eventsCol = _db.GetCollection<EventModel>("Events");
                    var evtDoc = await eventsCol.Find(e => e.Id == eventIdQ).FirstOrDefaultAsync();
                    if (evtDoc == null || string.IsNullOrWhiteSpace(evtDoc.BannerPath) || !System.IO.File.Exists(evtDoc.BannerPath))
                    {
                        // Return 1x1 transparent PNG if missing
                        byte[] placeholder = Convert.FromBase64String(
                            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR4nGNgYGBgAAAABQABDQottAAAAABJRU5ErkJggg==");
                        res.ContentType = "image/png";
                        res.ContentLength64 = placeholder.Length;
                        await res.OutputStream.WriteAsync(placeholder, 0, placeholder.Length);
                        res.OutputStream.Close();
                        return;
                    }
                    try
                    {
                        var ext = System.IO.Path.GetExtension(evtDoc.BannerPath).ToLowerInvariant();
                        res.ContentType = ext switch
                        {
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            ".gif" => "image/gif",
                            _ => "application/octet-stream"
                        };
                        byte[] imgBytes = await System.IO.File.ReadAllBytesAsync(evtDoc.BannerPath);
                        res.ContentLength64 = imgBytes.Length;
                        await res.OutputStream.WriteAsync(imgBytes, 0, imgBytes.Length);
                        res.OutputStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Error(res, "banner read failed: " + ex.Message);
                    }
                    return;
                }

                // GET criteria-level scores for a single judge + contestant
                if (path.Equals("/api/judge-scores", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "GET")
                {
                    var eventIdQ = req.QueryString["eventId"];
                    var judgeIdQ = req.QueryString["judgeId"];
                    var contestantIdQ = req.QueryString["contestantId"];

                    if (string.IsNullOrWhiteSpace(eventIdQ) ||
                        string.IsNullOrWhiteSpace(judgeIdQ) ||
                        string.IsNullOrWhiteSpace(contestantIdQ))
                    {
                        BadRequest(res, "eventId, judgeId, contestantId required");
                        return;
                    }

                    var col = _db.GetCollection<ScoreEntry>("Scores"); // adjust if your collection name differs
                    var list = await col.Find(s => s.EventId == eventIdQ
                                                && s.JudgeId == judgeIdQ
                                                && s.ContestantId == contestantIdQ)
                                        .SortBy(s => s.CreatedAt)
                                        .ToListAsync();

                    await JsonAsync(res, list);
                    return;
                }

                // LEADERBOARD
                if (path.Equals("/api/leaderboard", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrWhiteSpace(eventId))
                    {
                        BadRequest(res, "eventId required");
                        return;
                    }
                    var data = await _scoring.GetLeaderboardAsync(eventId);
                    await JsonAsync(res, data);
                    return;
                }

                NotFound(res);
            }
            catch (Exception ex)
            {
                Error(res, ex.Message);
            }
        }

        private async Task ServeStaticAsync(HttpListenerResponse res, string relativePath)
        {
            string full = Path.Combine(_staticRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(full))
            {
                NotFound(res);
                return;
            }

            string ext = Path.GetExtension(full).ToLowerInvariant();
            string contentType = ext switch
            {
                ".html" => "text/html; charset=utf-8",
                ".htm" => "text/html; charset=utf-8",
                ".js" => "application/javascript; charset=utf-8",
                ".css" => "text/css; charset=utf-8",
                ".json" => "application/json; charset=utf-8",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            res.ContentType = contentType;
            var bytes = await File.ReadAllBytesAsync(full);
            res.ContentLength64 = bytes.Length;
            await res.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            res.OutputStream.Close();
        }

        private static void Redirect(HttpListenerResponse res, string to)
        {
            res.StatusCode = 302;
            res.RedirectLocation = to;
            res.OutputStream.Close();
        }

        private static void NotFound(HttpListenerResponse res)
        {
            res.StatusCode = 404;
            WriteUtf8(res, "{\"error\":\"not found\"}");
        }

        private static void BadRequest(HttpListenerResponse res, string msg)
        {
            res.StatusCode = 400;
            WriteUtf8(res, "{\"error\":\"" + Escape(msg) + "\"}");
        }

        private static void Error(HttpListenerResponse res, string msg)
        {
            res.StatusCode = 500;
            WriteUtf8(res, "{\"error\":\"" + Escape(msg) + "\"}");
        }

        private static async Task JsonAsync(HttpListenerResponse res, object data)
        {
            res.ContentType = "application/json; charset=utf-8";
            string json = JsonSerializer.Serialize(data);
            await WriteUtf8Async(res, json);
        }

        private static void WriteUtf8(HttpListenerResponse res, string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            res.ContentLength64 = bytes.Length;
            res.OutputStream.Write(bytes, 0, bytes.Length);
            res.OutputStream.Close();
        }

        private static async Task WriteUtf8Async(HttpListenerResponse res, string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            res.ContentLength64 = bytes.Length;
            await res.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            res.OutputStream.Close();
        }

        private static string Escape(string s) => s.Replace("\"", "\\\"");

        public void Dispose() => Stop();

        // DTOs
        private class ScoreEntryDto
        {
            public string EventId { get; set; } = default!;
            public string ContestantId { get; set; } = default!;
            public string JudgeId { get; set; } = default!;
            public string PhaseId { get; set; } = default!;
            public string SegmentId { get; set; } = default!;
            public string CriteriaId { get; set; } = default!;
            public double RawValue { get; set; }
            public double MaxValue { get; set; } = 100;
        }
        private class JudgeLoginDto
        {
            public string EventId { get; set; } = default!;
            public string JudgeId { get; set; } = default!;
            public string? Pin { get; set; }
        }
        private class LiveStateDto
        {
            public string EventId { get; set; } = default!;
            public string? PhaseKey { get; set; }
            public string? SegmentKey { get; set; }
            public string? ContestantId { get; set; }
        }
        public class LiveState
        {
            public string EventId { get; set; } = default!;
            public string? PhaseKey { get; set; }
            public string? SegmentKey { get; set; }
            public string? ContestantId { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        private class JudgeHeartbeatDto
        {
            public string EventId { get; set; } = default!;
            public string JudgeId { get; set; } = default!;
        }
        private class UpdateActiveRequest
        {
            public string EventId { get; set; } = default!;
            public List<UpdateActiveItem>? Updates { get; set; }
        }
        private class UpdateActiveItem
        {
            public string ContestantId { get; set; } = default!;
            public bool IsActive { get; set; }
        }
    }
}