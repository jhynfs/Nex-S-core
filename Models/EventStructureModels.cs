using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace NexScore.Models
{
    public class EventStructureModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("eventId")]
        [BsonRepresentation(BsonType.ObjectId)] // store as ObjectId in Mongo, use string in C#
        public string EventId { get; set; }  // link to EventModel.Id

        [BsonElement("phases")]
        public List<PhaseModel> Phases { get; set; } = new List<PhaseModel>();
    }

}
