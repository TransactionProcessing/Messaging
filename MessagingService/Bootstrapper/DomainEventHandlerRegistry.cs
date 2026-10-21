namespace MessagingService.Bootstrapper;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BusinessLogic.EventHandling;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.DomainDrivenDesign.EventSourcing;
using Shared.EventStore.Aggregate;
using Shared.EventStore.EventHandling;

[ExcludeFromCodeCoverage]
public class DomainEventHandlerRegistry : ServiceRegistry
{
    public DomainEventHandlerRegistry(IConfiguration configuration)
    {
        Dictionary<String, String[]> eventHandlersConfiguration = new Dictionary<String, String[]>();

        IConfigurationSection section = configuration.GetSection("AppSettings:EventHandlerConfiguration");
        if (section.Exists())
        {
            section.Bind(eventHandlersConfiguration);
        }
        this.AddSingleton<EmailDomainEventHandler>();
        this.AddSingleton<SMSDomainEventHandler>();
        this.AddSingleton<Dictionary<String, String[]>>(eventHandlersConfiguration);

        this.AddSingleton<Func<Type, IDomainEventHandler>>(container => (type) =>
                                                                        {
                                                                            IDomainEventHandler handler = container.GetService(type) as IDomainEventHandler;
                                                                            return handler;
                                                                        });

        this.For<IDomainEventHandlerResolver>().Use<DomainEventHandlerResolver>()
            .Ctor<Dictionary<String, String[]>>().Is(eventHandlersConfiguration).Singleton();
        this.AddSingleton<IDomainEventFactory<IDomainEvent>, DomainEventFactory>();
    }
}
