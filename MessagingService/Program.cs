using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingService
{
    using System.Diagnostics.CodeAnalysis;
    using EmailMessage.DomainEvents;
    using EventStore.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.Subscriptions;
    using SMSMessage.DomainEvents;

    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Console.Title = "Messaging Service";

            //At this stage, we only need our hosting file for ip and ports
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                  .AddJsonFile("hosting.json", optional: true)
                                                                  .AddJsonFile("hosting.development.json", optional: true)
                                                                  .AddEnvironmentVariables().Build();

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureLogging(logging =>
                                         {
                                             logging.AddConsole();

                                         }).ConfigureWebHostDefaults(webBuilder =>
                                                                     {
                                                                         webBuilder.UseStartup<Startup>();
                                                                         webBuilder.UseConfiguration(config);
                                                                         webBuilder.UseKestrel();
                                                                     })
                       .ConfigureServices(services =>
                                                              {
                                                                  RequestSentToEmailProviderEvent e = new RequestSentToEmailProviderEvent(Guid.Parse("2AA2D43B-5E24-4327-8029-1135B20F35CE"), "", new List<String>(),
                                                                      "", "", true);

                                                                  RequestSentToSMSProviderEvent s = new RequestSentToSMSProviderEvent(Guid.NewGuid(), "", "","");
                                                                  
                                                                  TypeProvider.LoadDomainEventsTypeDynamically();

                                                                  services.AddHostedService<SubscriptionWorker>();
                                                              });
            return hostBuilder;
        }

    }
}
