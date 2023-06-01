using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingService
{
    using System.Diagnostics.CodeAnalysis;
    using Lamar.Microsoft.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //At this stage, we only need our hosting file for ip and ports
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(fi.Directory.FullName)
                                                                  .AddJsonFile("hosting.json", optional: false)
                                                                  .AddJsonFile("hosting.development.json", optional: true)
                                                                  .AddEnvironmentVariables().Build();

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.UseWindowsService();
            hostBuilder.UseLamar();
            hostBuilder.ConfigureLogging(logging =>
                                         {
                                             logging.AddConsole();

                                         }).ConfigureWebHostDefaults(webBuilder =>
                                                                     {
                                                                         webBuilder.UseStartup<Startup>();
                                                                         webBuilder.UseConfiguration(config);
                                                                         webBuilder.UseKestrel();
                                                                     });
            return hostBuilder;
        }
    }
}
