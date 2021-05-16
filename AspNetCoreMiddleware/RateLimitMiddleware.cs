using System.Threading.Tasks;
using AspNetCoreMiddleware.Services.Base;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AspNetCoreMiddleware
{
    /// <summary>
    /// Base middleware for consumption any IRateLimitService and execution of it's logic
    /// </summary>
    /// <typeparam name="T">IRateLimitService</typeparam>
    public class RateLimitMiddleware<T> where T : IRateLimitService
    {
        private readonly RequestDelegate next;
        private readonly T rateLimitService;

        public RateLimitMiddleware(T rateLimitService, RequestDelegate next)
        {
            this.next = next;
            this.rateLimitService = rateLimitService;
        }

        /// <summary>
        /// Invoke rate limit specific checks and block moving down in pipeline in case rate limits
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            int secToWait = rateLimitService.AllowedRequestsInSec(rateLimitService.GetKey(context));

            if (secToWait > 0)
            {
                context.Response.StatusCode = 429;

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    message = $"Rate limit exceeded. Try again in #{secToWait} seconds"
                }));

                return;
            }

            // Call the next delegate/middleware in the pipeline
            await next(context);
        }
    }
}