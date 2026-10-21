using HealthMonitoring.Client;
using KurrentDB.Client;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.OpenApi;
using Shared.Authorisation;
using Shared.Serialisation;
using System;
using System.Text.Json;

namespace MessagingService.Bootstrapper
{
    using Common;
    using Lamar;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using OpenIddict.Validation.AspNetCore;
    using Shared.EventStore.Extensions;
    using Shared.Middleware;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Http;
    using TransactionProcessing.ApiDefaults;

    [ExcludeFromCodeCoverage]
    public class MiddlewareRegistry : ServiceRegistry
    {
        private readonly IConfiguration configuration;

        public MiddlewareRegistry(IConfiguration configuration)
        {
            this.configuration = configuration;
            String connectionString = configuration.GetValue<String>("EventStoreSettings:ConnectionString");
            KurrentDBClientSettings eventStoreConnectionSettings = KurrentDBClientSettings.Create(connectionString);

            this.AddHealthChecks()
                .AddEventStore(eventStoreConnectionSettings,
                               name: "Eventstore",
                               failureStatus: HealthStatus.Unhealthy,
                               tags: new string[] { "db", "eventstore" });


            this.AddControllers();
            this.ConfigureAuthentication();

            bool logRequests = configuration.GetValue("MiddlewareLogging:LogRequests", true);
            bool logResponses = configuration.GetValue("MiddlewareLogging:LogResponses", true);
            LogLevel middlewareLogLevel = configuration.GetValue("MiddlewareLogging:MiddlewareLogLevel", LogLevel.Warning);

            RequestResponseMiddlewareLoggingConfig config = new(middlewareLogLevel, logRequests, logResponses);

            this.AddSingleton(config);

            this.ConfigureHttpJsonOptions(jsonOptions => JsonSerializerConfiguration.ConfigureMinimalApi(jsonOptions.SerializerOptions));

            this.AddHealthMonitoringRegistration(configuration);

            this.AddApiDefaults(options =>
            {
                options.Title = "Messaging Service";
                options.Description = "API to allow email and sms integrations";
            });

            this.AddOpenApi(options =>
            {
                options.AddSchemaTransformer(new DtoXmlDocumentationSchemaTransformer());
                options.AddOperationTransformer(new MessagingRequestExamplesOperationTransformer());
            });
        }

        private void ConfigureAuthentication() {
            this.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            this.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Same as your Authority
                    options.SetIssuer(new Uri(this.configuration.GetValue<String>("SecurityConfiguration:Authority")));

                    // Enables discovery and HTTP backchannel support
                    options.UseSystemNetHttp()
                        .ConfigureHttpClientHandler(handler =>
                        {
                            // DEV ONLY: bypass all certificate errors
                            handler.ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        });

                    // Register the ASP.NET Core integration
                    options.UseAspNetCore();

                    // Optionally set expected audience(s):
                    options.AddAudiences(this.configuration.GetValue<String>("SecurityConfiguration:ApiName"));

                });

            this.AddAuthorization();


            this.AddClientCredentialsOnlyPolicy();
            this.AddClientCredentialsHandler();
        }
    }
}

public static class JsonSerializerConfiguration
{
    public static void ConfigureMinimalApi(JsonSerializerOptions serializerOptions)
    {
        var defaultOptions = SystemTextJsonSerializer.GetDefaultJsonSerializerOptions();
        serializerOptions.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;
        serializerOptions.DictionaryKeyPolicy = defaultOptions.DictionaryKeyPolicy;
        serializerOptions.ReferenceHandler = defaultOptions.ReferenceHandler;
        serializerOptions.WriteIndented = defaultOptions.WriteIndented;
        serializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
        serializerOptions.Converters.Add(new NullableUtcDateTimeJsonConverter());
        
    }

    private sealed class UtcDateTimeJsonConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();
            return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime());
        }
    }

    private sealed class NullableUtcDateTimeJsonConverter : System.Text.Json.Serialization.JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var value = reader.GetDateTime();
            return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue == false)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.Value.Kind == DateTimeKind.Utc ? value.Value : value.Value.ToUniversalTime());
        }
    }
}
