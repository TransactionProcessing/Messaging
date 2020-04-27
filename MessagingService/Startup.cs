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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using BusinessLogic.Common;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using BusinessLogic.Services.EmailServices;
    using BusinessLogic.Services.EmailServices.Smtp2Go;
    using Common;
    using EmailMessageAggregate;
    using EventStore.ClientAPI;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NLog.Extensions.Logging;
    using Service.Services.Email.IntegrationTest;
    using Shared.DomainDrivenDesign.EventStore;
    using Shared.EntityFramework.ConnectionStringConfiguration;
    using Shared.EventStore.EventStore;
    using Shared.Extensions;
    using Shared.General;
    using Shared.Logger;
    using Shared.Repositories;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using ILogger = EventStore.ClientAPI.ILogger;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(webHostEnvironment.ContentRootPath)
                                                                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                                      .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", optional: true).AddEnvironmentVariables();

            Startup.Configuration = builder.Build();
            Startup.WebHostEnvironment = webHostEnvironment;
        }

        public static IConfigurationRoot Configuration { get; set; }

        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.ConfigureMiddlewareServices(services);

            services.AddTransient<IMediator, Mediator>();

            ConfigurationReader.Initialise(Startup.Configuration);
            String connString = Startup.Configuration.GetValue<String>("EventStoreSettings:ConnectionString");
            String connectionName = Startup.Configuration.GetValue<String>("EventStoreSettings:ConnectionName");
            Int32 httpPort = Startup.Configuration.GetValue<Int32>("EventStoreSettings:HttpPort");

            Boolean useConnectionStringConfig = Boolean.Parse(ConfigurationReader.GetValue("AppSettings", "UseConnectionStringConfig"));
            EventStoreConnectionSettings settings = EventStoreConnectionSettings.Create(connString, connectionName, httpPort);
            services.AddSingleton(settings);

            services.AddSingleton<Func<EventStoreConnectionSettings, IEventStoreConnection>>(cont => (connectionSettings) =>
            {
                return EventStoreConnection.Create(connectionSettings
                                                       .ConnectionString);
            });

            services.AddSingleton<Func<String, IEventStoreContext>>(cont => (connectionString) =>
            {
                EventStoreConnectionSettings connectionSettings =
                    EventStoreConnectionSettings.Create(connectionString, connectionName, httpPort);

                Func<EventStoreConnectionSettings, IEventStoreConnection> eventStoreConnectionFunc = cont.GetService<Func<EventStoreConnectionSettings, IEventStoreConnection>>();

                IEventStoreContext context =
                    new EventStoreContext(connectionSettings, eventStoreConnectionFunc);

                return context;
            });


            if (useConnectionStringConfig)
            {
                String connectionStringConfigurationConnString = ConfigurationReader.GetConnectionString("ConnectionStringConfiguration");
                services.AddSingleton<IConnectionStringConfigurationRepository, ConnectionStringConfigurationRepository>();
                services.AddTransient<ConnectionStringConfigurationContext>(c =>
                {
                    return new ConnectionStringConfigurationContext(connectionStringConfigurationConnString);
                });

                services.AddSingleton<IEventStoreContextManager, EventStoreContextManager>(c =>
                {
                    Func<String, IEventStoreContext> contextFunc = c.GetService<Func<String, IEventStoreContext>>();
                    IConnectionStringConfigurationRepository connectionStringConfigurationRepository =
                        c.GetService<IConnectionStringConfigurationRepository>();
                    return new EventStoreContextManager(contextFunc,
                                                        connectionStringConfigurationRepository);
                });
            }
            else
            {
                services.AddSingleton<IEventStoreContextManager, EventStoreContextManager>(c =>
                {
                    IEventStoreContext context = c.GetService<IEventStoreContext>();
                    return new EventStoreContextManager(context);
                });
                services.AddSingleton<IConnectionStringConfigurationRepository, ConfigurationReaderConnectionStringRepository>();
            }

            services.AddTransient<IEventStoreContext, EventStoreContext>();
            services.AddSingleton<IAggregateRepositoryManager, AggregateRepositoryManager>();
            services.AddSingleton<IAggregateRepository<EmailAggregate>, AggregateRepository<EmailAggregate>>();
            services.AddSingleton<IEmailDomainService, EmailDomainService>();
            
            this.RegisterEmailProxy(services);

            //services.AddSingleton<IModelFactory, ModelFactory>();
            //services.AddSingleton<Factories.IModelFactory, Factories.ModelFactory>();
            //services.AddSingleton<ISecurityServiceClient, SecurityServiceClient>();

            //// request & notification handlers
            services.AddTransient<ServiceFactory>(context =>
                                                  {
                                                      return t => context.GetService(t);
                                                  });

            services.AddSingleton<IRequestHandler<SendEmailRequest, String>, EmailRequestHandler>();

            services.AddSingleton<Func<String, String>>(container => (serviceName) =>
                                                                     {
                                                                         return ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
                                                                     });
            services.AddSingleton<HttpClient>();
        }

        /// <summary>
        /// Registers the email proxy.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterEmailProxy(IServiceCollection services)
        {
            // read the config setting 
            String emailProxy = ConfigurationReader.GetValue("AppSettings", "EmailProxy");

            if (emailProxy == "Smtp2Go")
            {
                services.AddSingleton<IEmailServiceProxy, Smtp2GoProxy>();
            }
            else
            {
                services.AddSingleton<IEmailServiceProxy, IntegrationTestEmailServiceProxy>();
            }
        }

        private void ConfigureMiddlewareServices(IServiceCollection services)
        {
            services.AddApiVersioning(
                                      options =>
                                      {
                                          // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                                          options.ReportApiVersions = true;
                                          options.DefaultApiVersion = new ApiVersion(1, 0);
                                          options.AssumeDefaultVersionWhenUnspecified = true;
                                          options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                                      });

            services.AddVersionedApiExplorer(
                                             options =>
                                             {
                                                 // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                                                 // note: the specified format code will format the version as "'v'major[.minor][-status]"
                                                 options.GroupNameFormat = "'v'VVV";

                                                 // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                                                 // can also be used to control the format of the API version in route templates
                                                 options.SubstituteApiVersionInUrl = true;
                                             });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                // add a custom operation filter which sets default values
                c.OperationFilter<SwaggerDefaultValues>();
                c.ExampleFilters();
            });

            services.AddSwaggerExamplesFromAssemblyOf<SwaggerJsonConverter>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //options.SaveToken = true;
                    options.Authority = ConfigurationReader.GetValue("SecurityConfiguration", "Authority");
                    options.Audience = ConfigurationReader.GetValue("SecurityConfiguration", "ApiName");
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = ConfigurationReader.GetValue("SecurityConfiguration", "ApiName"),
                        ValidIssuer = ConfigurationReader.GetValue("SecurityConfiguration", "Authority"),
                    };
                    options.IncludeErrorDetails = true;
                });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            services.AddMvcCore().AddApplicationPart(assembly).AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
                              IApiVersionDescriptionProvider provider)
        {
            String nlogConfigFilename = "nlog.config";

            if (env.IsDevelopment())
            {
                nlogConfigFilename = $"nlog.{env.EnvironmentName}.config";
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.ConfigureNLog(Path.Combine(env.ContentRootPath, nlogConfigFilename));
            loggerFactory.AddNLog();

            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("EstateManagement");

            Logger.Initialise(logger);

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
                             });
            app.UseSwagger();

            app.UseSwaggerUI(
                             options =>
                             {
                                 // build a swagger endpoint for each discovered API version
                                 foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                                 {
                                     options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                                 }
                             });
        }
    }
}
