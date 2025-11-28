using NexScore.Utils;

namespace NexScore
{
    public static class ApiBase
    {
        public static string Get(int defaultPort = 5100)
        {
            var saved = SettingsService.LoadAdminBaseUrl($"http://localhost:{defaultPort}").Trim();
            if (NetUtil.TryNormalizeAdminBaseUrl(saved, out var norm, out var _, defaultPort))
                return norm.TrimEnd('/');
            return $"http://localhost:{defaultPort}";
        }
    }
}