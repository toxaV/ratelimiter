using AspNetCoreMiddleware.Settings.Base;

namespace AspNetCoreMiddleware.Settings
{
    public class ApiKeyLimitSettings : IRateLimitSettings
    {
        /// <summary>
        /// Allowed attempts in seconds
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Seconds
        /// </summary>
        public int Seconds { get; set; }
    }
}