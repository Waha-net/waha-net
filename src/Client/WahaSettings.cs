namespace Waha
{
    /// <summary>
    /// Represents the settings required to configure the Waha API client.
    /// </summary>
    public record WahaSettings
    {
        /// <summary>
        /// Gets or sets the endpoint URI for the Waha API.
        /// </summary>
        public Uri Endpoint { get; set; }
    }
}