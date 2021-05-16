using Microsoft.AspNetCore.Http;

namespace AspNetCoreMiddleware.Services.Base
{
    public interface IRateLimitService
    {
        /// <summary>
        /// Check when request is allowed to go through
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        /// Amount of seconds need to wait until request can be executed.
        /// 0 - Can be executed immediately
        /// >0 - Amount of seconds to wait
        /// </returns>
        int AllowedRequestsInSec(string key);

        /// <summary>
        /// Get key that will be applied to rates
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Key</returns>
        string GetKey(HttpContext context);
    }
}