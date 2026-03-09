using MessagingService.DataTransferObjects;
using SimpleResults;

namespace MessagingService.Bootstrapper;

using BusinessLogic.RequestHandlers;
using BusinessLogic.Requests;
using Google.Api;
using Lamar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.General;
using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class MediatorRegistry : ServiceRegistry
{
    public MediatorRegistry()
    {
        this.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MessagingRequestHandler).Assembly));
        this.AddSingleton<Func<String, String>>(container => (serviceName) => ConfigurationReader.GetBaseServerUri(serviceName).OriginalString);
    }
}