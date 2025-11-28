using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexScore.Models
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string EventId { get; set; } // Human code (not used for linking in your current scoring)

        [BsonElement("eventName")]
        public string EventName { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("venue")]
        public string Venue { get; set; }

        [BsonElement("organizers")]
        public string Organizers { get; set; }

        [BsonElement("eventDate")]
        public DateTime EventDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("bannerPath")]
        public string? BannerPath { get; set; }
    }

    public class PhaseModel
    {
        [BsonElement("sequence")]
        public int? Sequence { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("weight")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Weight { get; set; }
        public bool IsIndependent { get; set; }

        [BsonElement("segments")]
        public List<SegmentModel> Segments { get; set; } = new();
    }

    public class SegmentModel
    {
        [BsonElement("sequence")]
        public int Sequence { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("weight")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Weight { get; set; }

        [BsonElement("criteria")]
        public List<CriteriaModel> Criteria { get; set; } = new();
    }

    public class CriteriaModel
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("weight")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Weight { get; set; }
    }

    public class JudgeModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("eventId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EventId { get; set; }

        [BsonElement("judgeId")]
        public string JudgeId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("number")]
        public string Number { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("ipAddress")]
        public string IPAddress { get; set; }
    }

    public class ContestantModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("eventId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EventId { get; set; }

        [BsonElement("number")]
        public int Number { get; set; }

        [BsonElement("fullName")]
        public string FullName { get; set; }

        [BsonElement("representing")]
        public string Representing { get; set; }

        [BsonElement("gender")]
        public string? Gender { get; set; }

        [BsonElement("age")]
        public int? Age { get; set; }

        [BsonElement("advocacy")]
        public string? Advocacy { get; set; }

        [BsonElement("photoPath")]
        public string? PhotoPath { get; set; }

        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // Transitional ScoreEntry mapping:
    // _id stored as ObjectId (sample doc shows this)
    // EventId / ContestantId stored as simple string (sample doc shows uppercase field names with values that LOOK like ObjectId strings)
    // We DO NOT force them to ObjectId right now to avoid filter misses if stored as strings.
    [BsonIgnoreExtraElements]
    public class ScoreEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Stored field name is "EventId" (uppercase) because you did not annotate with [BsonElement]
        public string EventId { get; set; } = default!;

        public string ContestantId { get; set; } = default!;
        public string JudgeId { get; set; } = default!;
        public string PhaseId { get; set; } = default!;
        public string SegmentId { get; set; } = default!;
        public string CriteriaId { get; set; } = default!;
        public double RawValue { get; set; }
        public double MaxValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsFinalized { get; set; } = false;
    }

    [BsonIgnoreExtraElements]
    public class AggregatedScore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string EventId { get; set; } = default!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ContestantId { get; set; } = default!;

        public double EventScore { get; set; }
        public Dictionary<string, double> PhaseScores { get; set; } = new();
        public Dictionary<string, double> IndependentPhaseScores { get; set; } = new();
        public DateTime ComputedAt { get; set; } = DateTime.UtcNow;
        public int Rank { get; set; }
        public string? TieBreakInfo { get; set; }
    }
}