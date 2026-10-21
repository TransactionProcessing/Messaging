using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MessagingService.Bootstrapper;
using MessagingService.Common;
using MessagingService.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Sentry.Extensibility;
using Shared.Extensions;
using Shared.General;
using Shared.Logger;
using Shared.Middleware;
using Shared.Serialisation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using TransactionProcessing.ApiDefaults;

namespace MessagingService;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                   ?? AppContext.BaseDirectory;
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = assemblyDirectory
        });

        builder.Host.UseWindowsService();
        builder.Logging.AddConsole();
        builder.Logging.AddNLog();

        ConfigureConfiguration(builder.Configuration, builder.Environment, assemblyDirectory);
        ConfigurationReader.Initialise(builder.Configuration);
        ConfigureSentry(builder);

        builder.Services.AddHostedService<SubscriptionPreWarmHostedService>();

        builder.Host.UseLamar((_, services) =>
        {
            IConfiguration configuration = builder.Configuration;
            services.IncludeRegistry(new MediatorRegistry());
            services.IncludeRegistry(new RepositoryRegistry(configuration));
            services.IncludeRegistry(new MiddlewareRegistry(configuration));
            services.IncludeRegistry(new DomainServiceRegistry());
            services.IncludeRegistry(new DomainEventHandlerRegistry(configuration));
            services.IncludeRegistry(new MessagingProxyRegistry(configuration));
            services.IncludeRegistry(new SerialiserRegistry());
        });

        WebApplication app = builder.Build();
        ConfigureApplication(app);
        app.Run();
    }

    private static void ConfigureLogging()
    {
        string nlogConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");

        LogManager.Setup(b =>
        {
            b.SetupLogFactory(setup =>
            {
                setup.AddCallSiteHiddenAssembly(typeof(NlogLogger).Assembly);
                setup.AddCallSiteHiddenAssembly(typeof(Shared.Logger.Logger).Assembly);
                setup.AddCallSiteHiddenAssembly(typeof(TenantMiddleware).Assembly);
            });
            b.LoadConfigurationFromFile(nlogConfigPath);
        });
    }

    private static void ConfigureConfiguration(IConfigurationManager configuration,
                                               IHostEnvironment environment,
                                               string basePath)
    {
        RemoveDefaultAppSettingsSources(configuration, environment.EnvironmentName);

        configuration.SetBasePath(basePath)
            .AddJsonFile("hosting.json", optional: true)
            .AddJsonFile($"hosting.{environment.EnvironmentName}.json", optional: true)
            .AddJsonFile("/home/txnproc/config/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"/home/txnproc/config/appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("/home/txnproc/config/appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["HealthMonitoring:Service:Version"] = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0.0"
            });
    }

    private static void RemoveDefaultAppSettingsSources(IConfigurationManager configuration,
                                                        string environmentName)
    {
        string environmentSettingsPath = $"appsettings.{environmentName}.json";

        for (int index = configuration.Sources.Count - 1; index >= 0; index--)
        {
            if (configuration.Sources[index] is JsonConfigurationSource jsonSource
                && (jsonSource.Path == "appsettings.json" || jsonSource.Path == environmentSettingsPath))
            {
                configuration.Sources.RemoveAt(index);
            }
        }
    }

    private static void ConfigureSentry(WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;
        IHostEnvironment environment = builder.Environment;
        IConfigurationSection sentrySection = configuration.GetSection("SentryConfiguration");

        if (sentrySection.Exists() && environment.IsDevelopment() == false)
        {
            builder.WebHost.UseSentry(options =>
            {
                options.Dsn = configuration["SentryConfiguration:Dsn"];
                options.SendDefaultPii = true;
                options.MaxRequestBodySize = RequestSize.Always;
                options.CaptureBlockingCalls = configuration.GetValue("SentryConfiguration:CaptureBlockingCalls", false);
                options.IncludeActivityData = configuration.GetValue("SentryConfiguration:IncludeActivityData", false);
                options.Release = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            });
        }
    }

    private static void ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        Microsoft.Extensions.Logging.ILogger logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("MessagingService");
        Shared.Logger.Logger.Initialise(logger);
        ((IConfigurationRoot)app.Configuration).LogConfiguration(Shared.Logger.Logger.LogWarning);

        IStringSerialiser serialiser = app.Services.GetRequiredService<IStringSerialiser>();
        StringSerialiser.Initialise(serialiser);

        foreach (KeyValuePair<Type, string> type in TypeMap.Map)
        {
            Shared.Logger.Logger.LogInformation($"Type name {type.Value} mapped to {type.Key.Name}");
        }

        app.UseMiddleware<TenantMiddleware>();
        app.AddRequestResponseLogging();
        app.AddExceptionHandler();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapEmailEndpoints();
        app.MapSMSEndpoints();
        app.MapDomainEventEndpoint();

        app.MapHealthChecks("health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = Shared.HealthChecks.HealthCheckMiddleware.WriteResponse
        })
            .WithName("HealthCheck")
            .WithMetadata(
                new EndpointSummaryMetadata("Checks the health of the Messaging Service"),
                new EndpointDescriptionMetadata("Returns the health status of the service and its configured dependency checks."));
        app.MapHealthChecks("healthui", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
        })
            .WithName("HealthCheckUi")
            .WithMetadata(
                new EndpointSummaryMetadata("Returns health details for the health-check UI"),
                new EndpointDescriptionMetadata("Returns the health-check data formatted for the health monitoring UI."));

        app.UseApiDefaults();
    }

    private sealed record EndpointSummaryMetadata(string Summary) : IEndpointSummaryMetadata;

    private sealed record EndpointDescriptionMetadata(string Description) : IEndpointDescriptionMetadata;
}
