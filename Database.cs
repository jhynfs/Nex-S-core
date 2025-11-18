using MongoDB.Driver;
using NexScore.Models;
using System;

namespace NexScore
{
    public static class Database
    {
        private static IMongoDatabase? _database;

        public static void Initialize()
        {
            string connectionString = "mongodb://localhost:27017";
            string dbName = "NexScoreDB";

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
        }

        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            if (_database == null)
                throw new InvalidOperationException("Database not initialized. Call Database.Initialize() first.");

            return _database.GetCollection<T>(name);
        }

        public static IMongoCollection<AdminModel> Admins => GetCollection<AdminModel>("Admins");
        public static IMongoCollection<EventModel> Events => GetCollection<EventModel>("Events");
        public static IMongoCollection<JudgeModel> Judges => GetCollection<JudgeModel>("Judges");

        // Add other collections used by pages
        public static IMongoCollection<ContestantModel> Contestants => GetCollection<ContestantModel>("Contestants");
        public static IMongoCollection<EventStructureModel> EventStructures => GetCollection<EventStructureModel>("EventStructures");
    }
}