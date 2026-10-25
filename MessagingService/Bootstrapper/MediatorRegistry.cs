namespace MessagingService.Bootstrapper;

using BusinessLogic.RequestHandlers;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Shared.General;
using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class MediatorRegistry : ServiceRegistry
{
    public MediatorRegistry()
    {
        this.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(MessagingRequestHandler).Assembly);
            cfg.LicenseKey = Environment.GetEnvironmentVariable("MEDIATR_LICENCEKEY");
        });
        this.AddSingleton<Func<String, String>>(container => (serviceName) => ConfigurationReader.GetBaseServerUri(serviceName).OriginalString);
    }
}