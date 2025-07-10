using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MessagingService
{
    using Bootstrapper;
    using Common;
    using EmailMessage.DomainEvents;
    using EventStore.Client;
    using HealthChecks.UI.Client;
    using Lamar;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using NLog;
    using NLog.Extensions.Logging;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.EventHandling;
    using Shared.EventStore.SubscriptionWorker;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Shared.Middleware;
    using SMSMessage.DomainEvents;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;

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
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("MessagingService");

            Shared.Logger.Logger.Initialise(logger);
            Startup.Configuration.LogConfiguration(Shared.Logger.Logger.LogWarning);

            foreach (KeyValuePair<Type, String> type in TypeMap.Map)
            {
                Shared.Logger.Logger.LogInformation($"Type name {type.Value} mapped to {type.Key.Name}");
            }

            ConfigurationReader.Initialise(Startup.Configuration);
            app.UseMiddleware<TenantMiddleware>();
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
}
