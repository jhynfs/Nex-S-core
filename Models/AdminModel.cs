using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexScore.Models
{
    public class AdminModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PinHash { get; set; }
        public string RecoveryCodeHash { get; set; }
    }
}
