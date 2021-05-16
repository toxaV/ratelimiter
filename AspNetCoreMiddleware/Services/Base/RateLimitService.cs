using System;
using System.Collections.Generic;
using AspNetCoreMiddleware.Algorithm;
using AspNetCoreMiddleware.Settings.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AspNetCoreMiddleware.Services.Base
{
    public abstract class RateLimitService<T>:  IRateLimitService where T: IRateLimitSettings
    {
        private readonly IMemoryCache cache;

        protected readonly IOptionsMonitor<T> settingsMonitor;

        private readonly RateLimitAlgorithm algorithm = new RateLimitAlgorithm();

        /// <summary>
        /// Seconds
        /// </summary>
        public abstract int Seconds { get; set; }

        /// <summary>
        /// Allowed attempts in seconds
        /// </summary>
        public abstract int Attempts { get; set; }

        protected RateLimitService(
            IMemoryCache memoryCache, 
            IOptionsMonitor<T> settingsMonitor)
        {
            this.cache = memoryCache;
            this.settingsMonitor = settingsMonitor;
        }

        /// <summary>
        /// Check when request is allowed to go through
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        /// Amount of seconds need to wait until request can be executed.
        /// 0 - Can be executed immediately
        /// >0 - Amount of seconds to wait
        /// </returns>
        public int AllowedRequestsInSec(string key)
        {
            var userRates = cache.Get<Queue<DateTime>>(key);

            var result = algorithm.RequestAllowedInSec(Seconds, Attempts, DateTime.UtcNow, ref userRates);

            cache.Set(key, userRates, DateTimeOffset.Now.AddMinutes(10));

            return result;
        }

        /// <summary>
        /// Get key that will be applied to rates
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Key</returns>
        public abstract string GetKey(HttpContext context);
    }
}