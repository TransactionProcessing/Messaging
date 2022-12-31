namespace MessagingService.Bootstrapper;

using BusinessLogic.Services;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class DomainServiceRegistry : ServiceRegistry
{
    public DomainServiceRegistry()
    {
        this.AddSingleton<IMessagingDomainService, MessagingDomainService>();
    }
}