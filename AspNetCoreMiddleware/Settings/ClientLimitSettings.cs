using AspNetCoreMiddleware.Settings.Base;

namespace AspNetCoreMiddleware.Settings
{
    /// <summary>
    /// Client specific settings from configuration
    /// </summary>
    public class ClientLimitSettings : IRateLimitSettings
    {
        public int Attempts { get; set; }

        public int Seconds { get; set; }

        public Clients[] Clients { get; set; }
    }

    public class Clients
    {
        public string Name { get; set; }

        public int Attempts { get; set; }

        public int Seconds { get; set; }

        public Api[] Apis { get; set; }
    }

    public class Api
    {
        public string Name { get; set; }

        public int Attempts { get; set; }

        public int Seconds { get; set; }
    }
}