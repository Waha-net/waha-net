using Microsoft.Extensions.Hosting;

namespace Waha
{
    /// <summary>
    /// Provides a builder for configuring and creating instances of the Waha API client.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WahaApiClientBuilder"/> class.
    /// </remarks>
    /// <param name="hostBuilder">The host application builder.</param>
    /// <param name="serviceKey">The service key for the Waha API client.</param>
    public class WahaApiClientBuilder(IHostApplicationBuilder hostBuilder, string serviceKey)
    {
        /// <summary>
        /// Gets the host application builder.
        /// </summary>
        public IHostApplicationBuilder HostBuilder { get; } = hostBuilder;

        /// <summary>
        /// Gets the service key for the Waha API client.
        /// </summary>
        public string ServiceKey { get; } = serviceKey;
    }
}

