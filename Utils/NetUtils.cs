using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace NexScore.Utils
{
    public static class NetUtil
    {
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
                    if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                    if (IPAddress.IsLoopback(ip)) continue;

                    if (privateOnly && !IsPrivateIPv4(ip)) continue;
                    
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
            if (b[0] == 10) return true; 
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
            if (b[0] == 192 && b[1] == 168) return true;
            return false;
        }
    }
}