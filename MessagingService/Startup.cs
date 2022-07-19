using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingService
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.Intrinsics;
    using System.Threading;
    using Bootstrapper;
    using BusinessLogic.Common;
    using BusinessLogic.EventHandling;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using BusinessLogic.Services.EmailServices;
    using BusinessLogic.Services.EmailServices.Smtp2Go;
    using BusinessLogic.Services.SMSServices;
    using BusinessLogic.Services.SMSServices.TheSMSWorks;
    using Common;
    using EmailMessage.DomainEvents;
    using EmailMessageAggregate;
    using EventStore.Client;
    using HealthChecks.UI.Client;
    using Lamar;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NLog.Extensions.Logging;
    using Service.Services.Email.IntegrationTest;
    using Service.Services.SMSServices.IntegrationTest;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EntityFramework.ConnectionStringConfiguration;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.EventHandling;
    using Shared.EventStore.EventStore;
    using Shared.EventStore.Extensions;
    using Shared.EventStore.SubscriptionWorker;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Shared.Repositories;
    using SMSMessage.DomainEvents;
    using SMSMessageAggregate;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(webHostEnvironment.ContentRootPath)
                                                                      .AddJsonFile("/home/txnproc/config/appsettings.json", true, true)
                                                                      .AddJsonFile($"/home/txnproc/config/appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true)
                                                                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                                      .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                                                      .AddEnvironmentVariables();

            Startup.Configuration = builder.Build();
            Startup.WebHostEnvironment = webHostEnvironment;
        }

        public static IConfigurationRoot Configuration { get; set; }

        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        internal static EventStoreClientSettings EventStoreClientSettings;

        public static void LoadTypes()
        {
            RequestSentToEmailProviderEvent e = new RequestSentToEmailProviderEvent(Guid.Parse("2AA2D43B-5E24-4327-8029-1135B20F35CE"), "", new List<String>(),
                                                                                    "", "", true);

            RequestSentToSMSProviderEvent s = new RequestSentToSMSProviderEvent(Guid.NewGuid(), "", "", "");

            TypeProvider.LoadDomainEventsTypeDynamically();
        }

        public static void ConfigureEventStoreSettings(EventStoreClientSettings settings)
        {
            settings.ConnectivitySettings = EventStoreClientConnectivitySettings.Default;
            settings.ConnectivitySettings.Address = new Uri(Startup.Configuration.GetValue<String>("EventStoreSettings:ConnectionString"));
            settings.ConnectivitySettings.Insecure = Startup.Configuration.GetValue<Boolean>("EventStoreSettings:Insecure");
            
            settings.DefaultCredentials = new UserCredentials(Startup.Configuration.GetValue<String>("EventStoreSettings:UserName"),
                                                              Startup.Configuration.GetValue<String>("EventStoreSettings:Password"));
            Startup.EventStoreClientSettings = settings;
        }

        public static Container Container;

        public void ConfigureContainer(ServiceRegistry services)
        {
            ConfigurationReader.Initialise(Startup.Configuration);
            
            services.IncludeRegistry<MediatorRegistry>();
            services.IncludeRegistry<RepositoryRegistry>();
            services.IncludeRegistry<MiddlewareRegistry>();
            services.IncludeRegistry<DomainServiceRegistry>();
            services.IncludeRegistry<DomainEventHandlerRegistry>();
            services.IncludeRegistry<MessagingProxyRegistry>();

            Startup.Container = new Container(services);

            Startup.ServiceProvider = services.BuildServiceProvider();
        }

        public static IServiceProvider ServiceProvider { get; set; }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            String nlogConfigFilename = "nlog.config";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.ConfigureNLog(Path.Combine(env.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();

            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("MessagingService");

            Logger.Initialise(logger);

            Action<String> loggerAction = message =>
                                          {
                                              Logger.LogInformation(message);
                                          };
            Startup.Configuration.LogConfiguration(loggerAction);
            
            foreach (KeyValuePair<Type, String> type in TypeMap.Map)
            {
                Logger.LogInformation($"Type name {type.Value} mapped to {type.Key.Name}");
            }

            ConfigurationReader.Initialise(Startup.Configuration);

            app.AddRequestLogging();
            app.AddResponseLogging();
            app.AddExceptionHandler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapControllers();
                                 endpoints.MapHealthChecks("health", new HealthCheckOptions()
                                                                     {
                                                                         Predicate = _ => true,
                                                                         ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                                                     });
                             });
            app.UseSwagger();

            app.UseSwaggerUI();

            app.PreWarm();
        }
    }

    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        static Action<TraceEventType, String, String> log = (tt, subType, message) => {
            String logMessage = $"{subType} - {message}";
            switch (tt)
            {
                case TraceEventType.Critical:
                    Logger.LogCritical(new Exception(logMessage));
                    break;
                case TraceEventType.Error:
                    Logger.LogError(new Exception(logMessage));
                    break;
                case TraceEventType.Warning:
                    Logger.LogWarning(logMessage);
                    break;
                case TraceEventType.Information:
                    Logger.LogInformation(logMessage);
                    break;
                case TraceEventType.Verbose:
                    Logger.LogDebug(logMessage);
                    break;
            }
        };

        static Action<TraceEventType, String> concurrentLog = (tt, message) => log(tt, "CONCURRENT", message);

        public static void PreWarm(this IApplicationBuilder applicationBuilder)
        {
            Startup.LoadTypes();

            //SubscriptionWorker worker = new SubscriptionWorker()
            var internalSubscriptionService = Boolean.Parse(ConfigurationReader.GetValue("InternalSubscriptionService"));

            if (internalSubscriptionService)
            {
                String eventStoreConnectionString = ConfigurationReader.GetValue("EventStoreSettings", "ConnectionString");
                Int32 inflightMessages = Int32.Parse(ConfigurationReader.GetValue("AppSettings", "InflightMessages"));
                Int32 persistentSubscriptionPollingInSeconds = Int32.Parse(ConfigurationReader.GetValue("AppSettings", "PersistentSubscriptionPollingInSeconds"));
                String filter = ConfigurationReader.GetValue("AppSettings", "InternalSubscriptionServiceFilter");
                String ignore = ConfigurationReader.GetValue("AppSettings", "InternalSubscriptionServiceIgnore");
                String streamName = ConfigurationReader.GetValue("AppSettings", "InternalSubscriptionFilterOnStreamName");
                Int32 cacheDuration = Int32.Parse(ConfigurationReader.GetValue("AppSettings", "InternalSubscriptionServiceCacheDuration"));

                ISubscriptionRepository subscriptionRepository = SubscriptionRepository.Create(eventStoreConnectionString, cacheDuration);

                ((SubscriptionRepository)subscriptionRepository).Trace += (sender, s) => Extensions.log(TraceEventType.Information, "REPOSITORY", s);

                // init our SubscriptionRepository
                subscriptionRepository.PreWarm(CancellationToken.None).Wait();

                var eventHandlerResolver = Startup.ServiceProvider.GetService<IDomainEventHandlerResolver>();

                SubscriptionWorker concurrentSubscriptions = SubscriptionWorker.CreateConcurrentSubscriptionWorker(Startup.EventStoreClientSettings, eventHandlerResolver, subscriptionRepository, inflightMessages, persistentSubscriptionPollingInSeconds);

                concurrentSubscriptions.Trace += (_, args) => concurrentLog(TraceEventType.Information, args.Message);
                concurrentSubscriptions.Warning += (_, args) => concurrentLog(TraceEventType.Warning, args.Message);
                concurrentSubscriptions.Error += (_, args) => concurrentLog(TraceEventType.Error, args.Message);

                if (!String.IsNullOrEmpty(ignore))
                {
                    concurrentSubscriptions = concurrentSubscriptions.IgnoreSubscriptions(ignore);
                }

                if (!String.IsNullOrEmpty(filter))
                {
                    //NOTE: Not overly happy with this design, but
                    //the idea is if we supply a filter, this overrides ignore
                    concurrentSubscriptions = concurrentSubscriptions.FilterSubscriptions(filter)
                                                                     .IgnoreSubscriptions(null);

                }

                if (!String.IsNullOrEmpty(streamName))
                {
                    concurrentSubscriptions = concurrentSubscriptions.FilterByStreamName(streamName);
                }

                concurrentSubscriptions.StartAsync(CancellationToken.None).Wait();
            }
        }
    }
}
