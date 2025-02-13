using System.Linq;
using System.Net;
using System.Numerics;
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
                if (ipw.Contains("/"))
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
            var parts = cidr.Split('/');
            var baseAddress = IPAddress.Parse(parts[0]);
            var address = IPAddress.Parse(ipAddress);
            int bits = int.Parse(parts[1]);

            if (baseAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                uint mask = ~(uint.MaxValue >> bits);
                uint baseAddressInt = BitConverter.ToUInt32(baseAddress.GetAddressBytes(), 0);
                uint addressInt = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
                return (baseAddressInt & mask) == (addressInt & mask);
            }
            else if (baseAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                BigInteger mask = BigInteger.One << (128 - bits) - 1;
                BigInteger baseAddressInt = new BigInteger(baseAddress.GetAddressBytes());
                BigInteger addressInt = new BigInteger(address.GetAddressBytes());
                return (baseAddressInt & mask) == (addressInt & mask);
            }
            else
            {
                throw new ArgumentException("Invalid address family");
            }
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
