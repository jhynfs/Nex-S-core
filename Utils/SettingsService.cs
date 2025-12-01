using System;
using System.IO;

namespace NexScore.Utils
{
    public static class SettingsService
    {
        private static string AppDataDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NexScore");

        private static string AdminBaseUrlPath => Path.Combine(AppDataDir, "admin-base-url.txt");

        public static string LoadAdminBaseUrl(string fallback = "http://localhost:5000")
        {
            try
            {
                if (File.Exists(AdminBaseUrlPath))
                {
                    var txt = File.ReadAllText(AdminBaseUrlPath).Trim();
                    if (!string.IsNullOrWhiteSpace(txt)) return txt.TrimEnd('/');
                }
            }
            catch {  }
            return fallback;
        }

        public static void SaveAdminBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return;
            var clean = baseUrl.Trim().TrimEnd('/');
            try
            {
                Directory.CreateDirectory(AppDataDir);
                File.WriteAllText(AdminBaseUrlPath, clean);
            }
            catch
            {
            }
        }
    }
}