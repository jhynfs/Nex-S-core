using System;
using System.IO;

namespace NexScore.Helpers
{
    public static class PathHelpers
    {
        public static string NexScoreRoot => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NexScore");

        public static string ContestantPhotosFolder => Path.Combine(NexScoreRoot, "Contestants");
        public static string EventsFolder => Path.Combine(NexScoreRoot, "Events");

        public static string MakeSafeFilename(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var safe = string.Join("_", input.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
            return safe.Replace(' ', '_');
        }

        public static string AbsoluteFromRelative(string relative)
        {
            if (string.IsNullOrWhiteSpace(relative)) return "";
            var cleaned = relative.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.Combine(NexScoreRoot, cleaned);
        }
    }
}