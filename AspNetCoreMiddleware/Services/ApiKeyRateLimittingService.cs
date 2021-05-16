using AspNetCoreMiddleware.Services.Base;
using AspNetCoreMiddleware.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AspNetCoreMiddleware.Services
{
    public class ApiKeyRateLimittingService: RateLimitService<ApiKeyLimitSettings>
    {
        public ApiKeyRateLimittingService(
            IMemoryCache memoryCache, 
            IOptionsMonitor<ApiKeyLimitSettings> settingsMonitor) : base(memoryCache, settingsMonitor)
        {
        }

        /// <summary>
        /// Seconds
        /// </summary>
        public override int Seconds
        {
            get => settingsMonitor.CurrentValue.Seconds;
            set { }
        }

        /// <summary>
        /// Allowed attempts in seconds
        /// </summary>
        public override int Attempts
        {
            get => settingsMonitor.CurrentValue.Attempts;
            set { }
        }

        /// <summary>
        /// Get key that will be applied to rates
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Key</returns>
        public override string GetKey(HttpContext context)
        {
            return context.Request.Headers["api-key"];
        }
    }
}