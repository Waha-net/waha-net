using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Waha;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Provides extension methods for configuring and adding the Waha API client to the host application builder.
    /// </summary>
    public static class WahaExtension
    {
        private const string DEFAULT_CONFIG_SECTION_NAME = "Waha";
        private const string ENDPOINT_KEY = "Endpoint";
        private const string KEYED_SERVICE_SUFFIX = "_WahaApiClient_internal";

        /// <summary>
        /// Adds the Waha API client to the host application builder with the specified connection name and optional settings configuration.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        /// <param name="connectionName">The name of the connection string to use for configuring the Waha API client.</param>
        /// <param name="configureSettings">An optional action to configure additional Waha settings.</param>
        /// <returns>A <see cref="WahaApiClientBuilder"/> for further configuration.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="connectionName"/> is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
        public static WahaApiClientBuilder AddWahaApiClient(this IHostApplicationBuilder builder, string connectionName, string? serviceKey = null, Action<WahaSettings>? configureSettings = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionName, nameof(connectionName));
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            return AddWahaClientInternal(builder, DEFAULT_CONFIG_SECTION_NAME, connectionName, serviceKey, configureSettings);
        }

        /// <summary>
        /// Internal method to add the Waha API client to the host application builder.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        /// <param name="configurationSectionName">The name of the configuration section to bind settings from.</param>
        /// <param name="connectionName">The name of the connection string to use for configuring the Waha API client.</param>
        /// <param name="serviceKey">An optional service key for keyed service registration.</param>
        /// <param name="configureSettings">An optional action to configure additional Waha settings.</param>
        /// <returns>A <see cref="WahaApiClientBuilder"/> for further configuration.</returns>
        private static WahaApiClientBuilder AddWahaClientInternal(IHostApplicationBuilder builder, string configurationSectionName, string connectionName, string? serviceKey = null, Action<WahaSettings>? configureSettings = null)
        {
            WahaSettings settings = new();
            builder.Configuration.GetSection(configurationSectionName).Bind(settings);

            if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            {
                var connectionBuilder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };

                if (connectionBuilder.ContainsKey(ENDPOINT_KEY) && Uri.TryCreate(connectionBuilder[ENDPOINT_KEY].ToString(), UriKind.Absolute, out Uri? endpoint))
                {
                    settings.Endpoint = endpoint;
                }
            }

            configureSettings?.Invoke(settings);

            if (!string.IsNullOrWhiteSpace(serviceKey))
            {
                builder.Services.AddKeyedSingleton(serviceKey, (sp, _) => ConfigureWahaClient(sp));
            }
            else
            {
                builder.Services.AddSingleton(ConfigureWahaClient);

                serviceKey = $"{connectionName}{KEYED_SERVICE_SUFFIX}";
                builder.Services.AddKeyedSingleton(serviceKey, (sp, _) => ConfigureWahaClient(sp));
            }

            return new WahaApiClientBuilder(builder, serviceKey);

            IWahaApiClient ConfigureWahaClient(IServiceProvider serviceProvider)
            {
                WahaApiClient client;
                try
                {
                    if (settings is null || settings.Endpoint is null)
                    {
                        settings = WahaSettings.Default;
                    }

                    client = new WahaApiClient(new HttpClient { BaseAddress = settings.Endpoint });
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"A WahaApiClient could not be configured. Ensure valid connection information was provided in 'ConnectionStrings:{connectionName}' or either " +
                        $"{nameof(settings.Endpoint)} must be provided " +
                        $"in the '{configurationSectionName}' configuration section.", ex);
                }

                return client;
            }
        }
    }
}