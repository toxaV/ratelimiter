using System.Linq;
using System.Text;
using AspNetCoreMiddleware.Services.Base;
using AspNetCoreMiddleware.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AspNetCoreMiddleware.Services
{
    public class ClientLimittingService : RateLimitService<ClientLimitSettings>
    {
        public ClientLimittingService(
            IMemoryCache memoryCache,
            IOptionsMonitor<ClientLimitSettings> settingsMonitor) : base(memoryCache, settingsMonitor)
        {
        }

        /// <summary>
        /// Seconds
        /// </summary>
        public override int Seconds { get; set; }

        /// <summary>
        /// Allowed attempts in seconds
        /// </summary>
        public override int Attempts { get; set; }

        /// <summary>
        /// Get key that will be applied to rates
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Key</returns>
        public override string GetKey(HttpContext context)
        {
            StringBuilder apiKeyBuilder = new StringBuilder();

            var apiKey = context.Request.Headers["api-key"];

            apiKeyBuilder.Append(apiKey);

            // Check if there is a specific rule for this client
            var client = settingsMonitor.CurrentValue.Clients.FirstOrDefault(x => x.Name == apiKey)
                         ?? settingsMonitor.CurrentValue.Clients.FirstOrDefault(x => x.Name == "UnknownApiKey");

            // Check if there is a specific rule for specific /path
            var path = client.Apis.FirstOrDefault(x => x.Name == context.Request.Path);

            if (path != null)
            {
                Seconds = path.Seconds;
                Attempts = path.Attempts;

                apiKeyBuilder.Append($"-{path.Name}");
            }
            else
            {
                Seconds = client.Seconds;
                Attempts = client.Attempts;

                apiKeyBuilder.Append($"-non-specified-path");
            }

            return apiKeyBuilder.ToString();
        }
    }
}