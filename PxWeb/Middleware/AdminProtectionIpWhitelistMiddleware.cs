using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PxWeb.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AdminProtectionIpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AdminProtectionConfigurationOptions _adminProtectionConfigurationOptions;
        private readonly HashSet<string> _ipWhitelist = new HashSet<string>();


        public AdminProtectionIpWhitelistMiddleware(RequestDelegate next, IAdminProtectionConfigurationService adminProtectionConfigurationService)
        {
            _next = next;
            _adminProtectionConfigurationOptions = adminProtectionConfigurationService.GetConfiguration();
            List<string> ipWhitelist = _adminProtectionConfigurationOptions.IpWhitelist;
            foreach (string ip in ipWhitelist) _ipWhitelist.Add(ip);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            IPAddress? ip = httpContext.Connection.RemoteIpAddress;
            if (ip == null) return;
            var ipadr = ip.ToString();
            bool match = false;

            foreach (string ipw in _ipWhitelist)
            {
                if (ipw.Contains('/'))
                {
                    // CIDR
                    match = _ipWhitelist.Any(cidr => IsInRange(ipadr, ipw));
                }
                else
                {
                    match = _ipWhitelist.Contains(ipadr);
                }
                if (match) break;
            }

            if (!match)
            {

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                Console.Write($"Unauthorized access from IP: {ip}");
                return;
            }
            await _next(httpContext);


        }

        public static bool IsInRange(string ipAddress, string cidr)
        {
            string[] parts = cidr.Split('/');
            if (parts.Length != 2) return false;

            IPAddress? networkAddress;
            if (!IPAddress.TryParse(parts[0], out networkAddress)) return false;
            if (!int.TryParse(parts[1], out int prefixLength)) return false;

            IPAddress? ip;
            if (!IPAddress.TryParse(ipAddress, out ip)) return false;

            if (networkAddress.AddressFamily != ip.AddressFamily) return false;

            byte[] ipBytes = ip.GetAddressBytes();
            byte[] networkBytes = networkAddress.GetAddressBytes();

            int fullBytes = prefixLength / 8;
            int remainingBits = prefixLength % 8;

            for (int i = 0; i < fullBytes; i++)
            {
                if (ipBytes[i] != networkBytes[i]) return false;
            }

            if (remainingBits > 0)
            {
                int mask = (byte)(0xFF << (8 - remainingBits));
                if ((ipBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask)) return false;
            }

            return true;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AdminProtectionIpWhitelistMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminProtectionIpWhitelist(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminProtectionIpWhitelistMiddleware>();
        }
    }
}
