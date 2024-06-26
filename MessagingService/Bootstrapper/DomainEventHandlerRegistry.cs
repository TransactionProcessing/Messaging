﻿namespace MessagingService.Bootstrapper;

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
    public DomainEventHandlerRegistry()
    {
        Dictionary<String, String[]> eventHandlersConfiguration = new Dictionary<String, String[]>();

        if (Startup.Configuration != null)
        {
            IConfigurationSection section = Startup.Configuration.GetSection("AppSettings:EventHandlerConfiguration");

            if (section != null)
            {
                Startup.Configuration.GetSection("AppSettings:EventHandlerConfiguration").Bind(eventHandlersConfiguration);
            }
        }
        this.AddSingleton<EmailDomainEventHandler>();
        this.AddSingleton<SMSDomainEventHandler>();
        this.AddSingleton<Dictionary<String, String[]>>(eventHandlersConfiguration);

        this.AddSingleton<Func<Type, IDomainEventHandler>>(container => (type) =>
                                                                        {
                                                                            IDomainEventHandler handler = container.GetService(type) as IDomainEventHandler;
                                                                            return handler;
                                                                        });

        this.For<IDomainEventHandlerResolver>().Use<DomainEventHandlerResolver>().Named("Main")
            .Ctor<Dictionary<String, String[]>>().Is(eventHandlersConfiguration).Singleton();
        this.AddSingleton<IDomainEventFactory<IDomainEvent>, DomainEventFactory>();
    }
}