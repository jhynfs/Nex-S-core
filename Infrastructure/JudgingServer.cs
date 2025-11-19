using MongoDB.Driver;
using NexScore.Models;
using NexScore.Services;
using System;
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

        public JudgingServer(IMongoDatabase db, string staticRoot)
        {
            _db = db;
            _scoring = new ScoringService(db);
            _staticRoot = staticRoot;
        }

        public void Start(int port = 5100)
        {
            if (_running) return;

            // Ensure indexes ONCE at startup (safe if it already exists)
            EnsureIndexes();

            string prefix = $"http://+:{port}/";
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            _running = true;
            _ = Task.Run(() => AcceptLoop(_cts.Token));
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
                var opts = new CreateIndexOptions
                {
                    Unique = true,
                    Name = "uniq_score_event_contestant_judge_criteria"
                };
                scores.Indexes.CreateOne(new CreateIndexModel<ScoreEntry>(keys, opts));
            }
            catch
            {
                // Swallow index creation errors (already exists, permissions, etc.) to avoid blocking startup.
            }
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
                HttpListenerContext ctx = null!;
                try
                {
                    ctx = await _listener.GetContextAsync();
                    _ = Task.Run(() => Handle(ctx));
                }
                catch when (ct.IsCancellationRequested) { return; }
                catch { /* ignore */ }
            }
        }

        private async Task Handle(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var res = ctx.Response;
            try
            {
                string path = req.Url?.AbsolutePath ?? "/";
                if (path == "/") { Redirect(res, "/JudgeClient/index.html"); return; }

                if (path.StartsWith("/JudgeClient/", StringComparison.OrdinalIgnoreCase))
                {
                    await ServeStaticAsync(res, path.Substring(1)); // remove leading '/'
                    return;
                }

                if (path.Equals("/api/structure", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrEmpty(eventId)) { BadRequest(res, "eventId required"); return; }
                    var s = await _db.GetCollection<EventStructureModel>("EventStructures")
                                     .Find(x => x.EventId == eventId).FirstOrDefaultAsync();
                    if (s == null) { NotFound(res); return; }
                    await JsonAsync(res, s);
                    return;
                }

                if (path.Equals("/api/contestants", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrEmpty(eventId)) { BadRequest(res, "eventId required"); return; }
                    var list = await _db.GetCollection<ContestantModel>("Contestants")
                                        .Find(c => c.EventId == eventId).SortBy(c => c.Number).ToListAsync();
                    await JsonAsync(res, list.Select(c => new { c.Id, c.Number, c.FullName, c.Representing }));
                    return;
                }

                if (path.Equals("/api/judges", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrEmpty(eventId)) { BadRequest(res, "eventId required"); return; }
                    var list = await _db.GetCollection<JudgeModel>("Judges")
                                        .Find(j => j.EventId == eventId).SortBy(j => j.Number).ToListAsync();
                    await JsonAsync(res, list.Select(j => new { j.Id, j.Name, j.Number, j.Title }));
                    return;
                }

                if (path.Equals("/api/score", StringComparison.OrdinalIgnoreCase) && req.HttpMethod == "POST")
                {
                    using var sr = new StreamReader(req.InputStream, req.ContentEncoding);
                    var raw = await sr.ReadToEndAsync();
                    var dto = JsonSerializer.Deserialize<ScoreEntryDto>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (dto == null) { BadRequest(res, "invalid json"); return; }

                    var now = DateTime.UtcNow;
                    var entry = new ScoreEntry
                    {
                        EventId = dto.EventId,
                        ContestantId = dto.ContestantId,
                        JudgeId = dto.JudgeId,
                        PhaseId = dto.PhaseId,
                        SegmentId = dto.SegmentId,
                        CriteriaId = (dto.CriteriaId ?? "").ToLowerInvariant(),
                        RawValue = dto.RawValue,
                        MaxValue = dto.MaxValue <= 0 ? 100 : dto.MaxValue,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    // Upsert via UpdateOne (no need for _id in replacement document)
                    var scores = _db.GetCollection<ScoreEntry>("Scores");
                    var keyFilter = Builders<ScoreEntry>.Filter.Where(e =>
                        e.EventId == entry.EventId &&
                        e.ContestantId == entry.ContestantId &&
                        e.JudgeId == entry.JudgeId &&
                        e.CriteriaId == entry.CriteriaId);

                    var update = Builders<ScoreEntry>.Update
                        .Set(e => e.PhaseId, entry.PhaseId)
                        .Set(e => e.SegmentId, entry.SegmentId)
                        .Set(e => e.CriteriaId, entry.CriteriaId)
                        .Set(e => e.RawValue, entry.RawValue)
                        .Set(e => e.MaxValue, entry.MaxValue)
                        .Set(e => e.UpdatedAt, now)
                        .SetOnInsert(e => e.EventId, entry.EventId)
                        .SetOnInsert(e => e.ContestantId, entry.ContestantId)
                        .SetOnInsert(e => e.JudgeId, entry.JudgeId)
                        .SetOnInsert(e => e.CreatedAt, now)
                        .SetOnInsert(e => e.IsFinalized, false);

                    await scores.UpdateOneAsync(keyFilter, update, new UpdateOptions { IsUpsert = true });

                    // Recompute aggregates for that contestant
                    await _scoring.RecomputeContestantAsync(entry.EventId, entry.ContestantId);

                    await JsonAsync(res, new { status = "ok" });
                    return;
                }

                if (path.Equals("/api/leaderboard", StringComparison.OrdinalIgnoreCase))
                {
                    string eventId = req.QueryString["eventId"];
                    if (string.IsNullOrEmpty(eventId)) { BadRequest(res, "eventId required"); return; }
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
            if (!File.Exists(full)) { NotFound(res); return; }
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
    }
}