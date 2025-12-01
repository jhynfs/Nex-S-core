using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NexScore.Models;

namespace NexScore.Services
{
    public class ScoringService
    {
        private readonly IMongoDatabase _db;

        public ScoringService(IMongoDatabase db)
        {
            _db = db;
        }

        private IMongoCollection<ScoreEntry> ScoreEntries => _db.GetCollection<ScoreEntry>("Scores");
        private IMongoCollection<AggregatedScore> AggregatedScores => _db.GetCollection<AggregatedScore>("AggregatedScores");
        private IMongoCollection<EventStructureModel> EventStructures => _db.GetCollection<EventStructureModel>("EventStructures");

        public async Task SubmitAsync(ScoreEntry entry)
        {
            if (entry.RawValue < 0 || entry.RawValue > entry.MaxValue)
                throw new InvalidOperationException("Score outside allowed range.");
            entry.UpdatedAt = DateTime.UtcNow;
            await ScoreEntries.InsertOneAsync(entry);
            await RecomputeContestantAsync(entry.EventId, entry.ContestantId);
        }

        public async Task RecomputeContestantAsync(string eventId, string contestantId)
        {
            contestantId = contestantId.ToString();
            eventId = eventId.ToString();

            var structure = await EventStructures.Find(s => s.EventId == eventId).FirstOrDefaultAsync();
            if (structure?.Phases == null || structure.Phases.Count == 0) return;

            var entries = await ScoreEntries.Find(e => e.EventId == eventId && e.ContestantId == contestantId).ToListAsync();
            var byCriteria = entries.GroupBy(e => e.CriteriaId).ToDictionary(g => g.Key, g => g.ToList());

            double eventScore = 0;
            var phaseScores = new Dictionary<string, double>();
            var indepScores = new Dictionary<string, double>();

            foreach (var phase in structure.Phases)
            {
                double phaseScore = 0;

                foreach (var seg in phase.Segments)
                {
                    double segScore = 0;
                    foreach (var crit in seg.Criteria)
                    {
                        var critId = BuildCriteriaId(phase, seg, crit);
                        if (!byCriteria.TryGetValue(critId, out var critEntries) || critEntries.Count == 0)
                            continue;

                        double avgRaw = critEntries.Average(e => e.RawValue);
                        double max = critEntries.First().MaxValue;
                        double normalized = max <= 0 ? 0 : (avgRaw / max);
                        double weightCrit = (double)crit.Weight / 100.0;
                        segScore += normalized * weightCrit;
                    }
                    double weightSeg = (double)seg.Weight / 100.0;
                    segScore *= weightSeg;
                    phaseScore += segScore;
                }

                if (phase.IsIndependent)
                {
                    indepScores[BuildPhaseId(phase)] = phaseScore;
                }
                else
                {
                    double weightPhase = (double)phase.Weight / 100.0;
                    double weighted = phaseScore * weightPhase;
                    phaseScores[BuildPhaseId(phase)] = weighted;
                    eventScore += weighted;
                }
            }

            var filter = Builders<AggregatedScore>.Filter.Where(a => a.EventId == eventId && a.ContestantId == contestantId);

            var existing = await AggregatedScores.Find(filter).FirstOrDefaultAsync();
            if (existing == null)
            {
                existing = new AggregatedScore
                {
                    EventId = eventId,
                    ContestantId = contestantId
                };
            }
            existing.EventId = eventId;
            existing.ContestantId = contestantId;
            existing.EventScore = eventScore;
            existing.PhaseScores = phaseScores;
            existing.IndependentPhaseScores = indepScores;
            existing.ComputedAt = DateTime.UtcNow;

            await AggregatedScores.ReplaceOneAsync(filter, existing, new ReplaceOptions { IsUpsert = true });
            await RecomputeRanksAsync(eventId);
        }

        public async Task RecomputeRanksAsync(string eventId)
        {
            eventId = eventId.ToString();
            var list = await AggregatedScores.Find(a => a.EventId == eventId).ToListAsync();
            var ordered = list
                .OrderByDescending(a => a.EventScore)
                .ThenByDescending(a => a.PhaseScores.Values.DefaultIfEmpty(0).Max())
                .ThenByDescending(a => a.IndependentPhaseScores.Values.DefaultIfEmpty(0).Sum())
                .ThenBy(a => a.ContestantId)
                .ToList();

            int rank = 1;
            foreach (var a in ordered)
            {
                a.Rank = rank++;
                a.TieBreakInfo = $"E={a.EventScore:0.000}";
            }

            var bulk = new List<WriteModel<AggregatedScore>>();
            foreach (var a in ordered)
                bulk.Add(new ReplaceOneModel<AggregatedScore>(Builders<AggregatedScore>.Filter.Eq(x => x.Id, a.Id), a));
            if (bulk.Count > 0)
                await AggregatedScores.BulkWriteAsync(bulk);
        }

        public async Task<object> GetLeaderboardAsync(string eventId)
        {
            eventId = eventId.ToString();
            var agg = await AggregatedScores.Find(a => a.EventId == eventId).SortBy(a => a.Rank).ToListAsync();
            return agg.Select(a => new
            {
                a.ContestantId,
                a.EventScore,
                a.Rank,
                Phases = a.PhaseScores,
                Independent = a.IndependentPhaseScores
            });
        }

        private string BuildCriteriaId(PhaseModel p, SegmentModel s, CriteriaModel c)
            => $"{BuildPhaseId(p)}::{s.Sequence}:{c.Name}".ToLowerInvariant();

        private string BuildPhaseId(PhaseModel p)
            => p.Sequence.HasValue ? $"P{p.Sequence.Value}" : $"IND-{Sanitize(p.Name)}";

        private string Sanitize(string? s) => string.IsNullOrWhiteSpace(s) ? "phase" : s.Trim().Replace(" ", "_").ToLowerInvariant();
    }
}