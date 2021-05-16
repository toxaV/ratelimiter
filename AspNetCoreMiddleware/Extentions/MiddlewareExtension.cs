using AspNetCoreMiddleware.Services;
using Microsoft.AspNetCore.Builder;

namespace AspNetCoreMiddleware.Extentions
{
    /// <summary>
    /// Middleware extenstion for injecting rate limits
    /// </summary>
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseClientRateLimiting(this IApplicationBuilder builder)
        {
            // Add other middleware that you want to inject into pipeline
            return builder
                // Check for Api-Key header and block request if there is no
                .UseMiddleware<ApiKeyMiddleware>()
                // Check for general settings, that do not depend on any client settings
                .UseMiddleware<RateLimitMiddleware<ApiKeyRateLimittingService>>()
                // Check for client specific settings and his limits
                .UseMiddleware<RateLimitMiddleware<ClientLimittingService>>();
        }
    }
}