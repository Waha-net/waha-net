namespace Waha
{
    /// <summary>
    /// Represents the settings required to configure the Waha API client.
    /// </summary>
    public record WahaSettings
    {
        private const string DEFAULT_WAHA_ENDPOINT = "localhost:3000";

        public static WahaSettings Default => new()
        {
            Endpoint = new Uri(DEFAULT_WAHA_ENDPOINT)
        };

        /// <summary>
        /// Gets or sets the endpoint URI for the Waha API.
        /// </summary>
        public Uri Endpoint { get; set; }
    }
}