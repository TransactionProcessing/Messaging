using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingService
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using Bootstrapper;
    using EmailMessage.DomainEvents;
    using EventStore.Client;
    using HealthChecks.UI.Client;
    using Lamar;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using NLog.Extensions.Logging;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.EventHandling;
    using Shared.EventStore.SubscriptionWorker;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using SMSMessage.DomainEvents;

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
                                                                         ResponseWriter = Shared.HealthChecks.HealthCheckMiddleware.WriteResponse
                                                                     });
                                 endpoints.MapHealthChecks("healthui", new HealthCheckOptions()
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

        static Action<TraceEventType, String> mainLog = (tt, message) => Extensions.log(tt, "MAIN", message);
        static Action<TraceEventType, String> orderedLog = (tt, message) => Extensions.log(tt, "ORDERED", message);

        public static void PreWarm(this IApplicationBuilder applicationBuilder)
        {
            Startup.LoadTypes();

            IConfigurationSection subscriptionConfigSection = Startup.Configuration.GetSection("AppSettings:SubscriptionConfiguration");
            SubscriptionWorkersRoot subscriptionWorkersRoot = new SubscriptionWorkersRoot();
            subscriptionConfigSection.Bind(subscriptionWorkersRoot);

            if (subscriptionWorkersRoot.InternalSubscriptionService)
            {

                String eventStoreConnectionString = ConfigurationReader.GetValue("EventStoreSettings", "ConnectionString");

                ISubscriptionRepository subscriptionRepository = SubscriptionRepository.Create(eventStoreConnectionString, subscriptionWorkersRoot.InternalSubscriptionServiceCacheDuration);
                ((SubscriptionRepository)subscriptionRepository).Trace += (sender,
                                                                           s) => Extensions.log(TraceEventType.Information, "REPOSITORY", s);

                // init our SubscriptionRepository
                subscriptionRepository.PreWarm(CancellationToken.None).Wait();

                List<SubscriptionWorker> workers = ConfigureSubscriptions(subscriptionRepository, subscriptionWorkersRoot);
                foreach (SubscriptionWorker subscriptionWorker in workers)
                {
                    subscriptionWorker.StartAsync(CancellationToken.None).Wait();
                }
            }
        }

        private static List<SubscriptionWorker> ConfigureSubscriptions(ISubscriptionRepository subscriptionRepository, SubscriptionWorkersRoot configuration)
        {
            List<SubscriptionWorker> workers = new List<SubscriptionWorker>();

            foreach (SubscriptionWorkerConfig configurationSubscriptionWorker in configuration.SubscriptionWorkers)
            {
                if (configurationSubscriptionWorker.Enabled == false)
                    continue;

                if (configurationSubscriptionWorker.IsOrdered)
                {
                    IDomainEventHandlerResolver eventHandlerResolver = Startup.Container.GetInstance<IDomainEventHandlerResolver>("Ordered");
                    SubscriptionWorker worker = SubscriptionWorker.CreateOrderedSubscriptionWorker(Startup.EventStoreClientSettings,
                                                                                                   eventHandlerResolver,
                                                                                                   subscriptionRepository,
                                                                                                   configuration.PersistentSubscriptionPollingInSeconds);
                    worker.Trace += (_,
                                     args) => Extensions.orderedLog(TraceEventType.Information, args.Message);
                    worker.Warning += (_,
                                       args) => Extensions.orderedLog(TraceEventType.Warning, args.Message);
                    worker.Error += (_,
                                     args) => Extensions.orderedLog(TraceEventType.Error, args.Message);
                    worker.SetIgnoreGroups(configurationSubscriptionWorker.IgnoreGroups);
                    worker.SetIgnoreStreams(configurationSubscriptionWorker.IgnoreStreams);
                    worker.SetIncludeGroups(configurationSubscriptionWorker.IncludeGroups);
                    worker.SetIncludeStreams(configurationSubscriptionWorker.IncludeStreams);
                    workers.Add(worker);

                }
                else
                {
                    for (Int32 i = 0; i < configurationSubscriptionWorker.InstanceCount; i++)
                    {
                        IDomainEventHandlerResolver eventHandlerResolver = Startup.Container.GetInstance<IDomainEventHandlerResolver>("Main");
                        SubscriptionWorker worker = SubscriptionWorker.CreateSubscriptionWorker(Startup.EventStoreClientSettings,
                                                                                                eventHandlerResolver,
                                                                                                subscriptionRepository,
                                                                                                configurationSubscriptionWorker.InflightMessages,
                                                                                                configuration.PersistentSubscriptionPollingInSeconds);

                        worker.Trace += (_,
                                         args) => Extensions.mainLog(TraceEventType.Information, args.Message);
                        worker.Warning += (_,
                                           args) => Extensions.mainLog(TraceEventType.Warning, args.Message);
                        worker.Error += (_,
                                         args) => Extensions.mainLog(TraceEventType.Error, args.Message);

                        worker.SetIgnoreGroups(configurationSubscriptionWorker.IgnoreGroups);
                        worker.SetIgnoreStreams(configurationSubscriptionWorker.IgnoreStreams);
                        worker.SetIncludeGroups(configurationSubscriptionWorker.IncludeGroups);
                        worker.SetIncludeStreams(configurationSubscriptionWorker.IncludeStreams);

                        workers.Add(worker);
                    }
                }
            }

            return workers;
        }
    }

    public class SubscriptionWorkersRoot
    {
        public Boolean InternalSubscriptionService { get; set; }
        public Int32 PersistentSubscriptionPollingInSeconds { get; set; }
        public Int32 InternalSubscriptionServiceCacheDuration { get; set; }
        public List<SubscriptionWorkerConfig> SubscriptionWorkers { get; set; }
    }

    public class SubscriptionWorkerConfig
    {
        public String WorkerName { get; set; }
        public String IncludeGroups { get; set; }
        public String IgnoreGroups { get; set; }
        public String IncludeStreams { get; set; }
        public String IgnoreStreams { get; set; }
        public Boolean Enabled { get; set; }
        public Int32 InflightMessages { get; set; }
        public Int32 InstanceCount { get; set; }
        public Boolean IsOrdered { get; set; }
    }
}
