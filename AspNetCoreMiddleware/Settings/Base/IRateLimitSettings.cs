namespace AspNetCoreMiddleware.Settings.Base
{
    public interface IRateLimitSettings
    {
        /// <summary>
        /// Allowed attempts in seconds
        /// </summary>
        int Attempts { get; set; }

        /// <summary>
        /// Seconds
        /// </summary>
        int Seconds { get; set; }
    }
}