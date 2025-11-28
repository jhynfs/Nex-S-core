using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace NexScore.Utils
{
    public static class NetUtil
    {
        // Validate and normalize admin base URL.
        // Accepts: 192.168.1.10, 192.168.1.10:5000, http://192.168.1.10, https://192.168.1.10:8443
        public static bool TryNormalizeAdminBaseUrl(string input, out string normalized, out string error, int defaultPort = 5000)
        {
            normalized = "";
            error = "";

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Empty address.";
                return false;
            }

            var s = input.Trim();

            // If scheme missing, default to http
            if (!s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                s = "http://" + s;
            }

            if (!Uri.TryCreate(s, UriKind.Absolute, out var uri))
            {
                error = "Invalid URL format.";
                return false;
            }

            // Require IPv4 host (per your requirement)
            if (!IPAddress.TryParse(uri.Host, out var ip))
            {
                error = "Host must be an IPv4 address.";
                return false;
            }

            if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                error = "Only IPv4 is supported.";
                return false;
            }

            // Reject link-local 169.254.x.x
            var bytes = ip.GetAddressBytes();
            if (bytes[0] == 169 && bytes[1] == 254)
            {
                error = "Link-local IPv4 (169.254.x.x) is not allowed.";
                return false;
            }

            var scheme = uri.Scheme.ToLowerInvariant() == "https" ? "https" : "http";
            var port = uri.IsDefaultPort ? defaultPort : uri.Port;

            normalized = $"{scheme}://{ip}:{port}";
            return true;
        }

        // Get all local IPv4 addresses that are up and likely usable on LAN
        public static List<IPAddress> GetLocalIPv4Candidates(bool privateOnly = true)
        {
            var list = new List<IPAddress>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

                var ipProps = ni.GetIPProperties();
                foreach (var ua in ipProps.UnicastAddresses)
                {
                    var ip = ua.Address;
                    if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue; // IPv4 only
                    if (IPAddress.IsLoopback(ip)) continue;

                    if (privateOnly && !IsPrivateIPv4(ip)) continue;
                    // Exclude link-local 169.254.x.x
                    var b = ip.GetAddressBytes();
                    if (b[0] == 169 && b[1] == 254) continue;

                    list.Add(ip);
                }
            }

            return list
                .Select(x => (x, key: x.ToString()))
                .GroupBy(x => x.key)
                .Select(g => g.First().x)
                .ToList();
        }

        public static string? GetDefaultLocalIPv4()
        {
            // Heuristic: prefer 192.168.x.x then 10.x.x.x then 172.16-31.x.x
            var ips = GetLocalIPv4Candidates(true);
            string? pick(params Func<IPAddress, bool>[] preds)
            {
                foreach (var p in preds)
                {
                    var hit = ips.FirstOrDefault(p);
                    if (hit != null) return hit.ToString();
                }
                return ips.FirstOrDefault()?.ToString();
            }

            bool Is192(IPAddress ip) => ip.GetAddressBytes()[0] == 192;
            bool Is10(IPAddress ip) => ip.GetAddressBytes()[0] == 10;
            bool Is172(IPAddress ip) => ip.GetAddressBytes()[0] == 172;

            return pick(Is192, Is10, Is172);
        }

        private static bool IsPrivateIPv4(IPAddress ip)
        {
            var b = ip.GetAddressBytes();
            if (b[0] == 10) return true; // 10.0.0.0/8
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true; // 172.16.0.0 - 172.31.255.255
            if (b[0] == 192 && b[1] == 168) return true; // 192.168.0.0/16
            return false;
        }
    }
}