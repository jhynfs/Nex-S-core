using System;
using System.IO;
using System.Text.Json;

namespace NexScore.Helpers
{
    public static class EventFileHelper
    {
        /// <summary>
        /// Saves the object as a JSON file inside the event's folder.
        /// The folder will be created if it doesn't exist.
        /// </summary>
        /// <param name="eventId">The event's unique ID</param>
        /// <param name="fileName">The name of the JSON file (e.g., "details.json")</param>
        /// <param name="data">The object to serialize</param>
        public static void SaveEventJson(string eventId, string fileName, object data)
        {
            if (string.IsNullOrEmpty(eventId))
                throw new ArgumentException("EventId cannot be null or empty.", nameof(eventId));

            // base folder
            string eventFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NexScore",
                "Events",
                eventId
            );

            // create folder if missing
            if (!Directory.Exists(eventFolder))
                Directory.CreateDirectory(eventFolder);

            // file path
            string filePath = Path.Combine(eventFolder, fileName);

            // write JSON (overwrite if exists)
            File.WriteAllText(filePath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Loads a JSON file from the event's folder and deserializes it.
        /// </summary>
        public static T? LoadEventJson<T>(string eventId, string fileName)
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NexScore",
                "Events",
                eventId,
                fileName
            );

            if (!File.Exists(filePath))
                return default;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
