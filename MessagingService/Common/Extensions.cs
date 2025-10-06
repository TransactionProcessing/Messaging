namespace MessagingService.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.EventStore.Aggregate;
using Shared.EventStore.EventHandling;
using Shared.EventStore.Extensions;
using Shared.EventStore.SubscriptionWorker;
using Shared.General;
using Shared.Logger;

[ExcludeFromCodeCoverage]
public static class Extensions
{
    static Action<TraceEventType, String, String> log = (tt,
                                                             subType,
                                                             message) => {
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
                                                                     default:
                                                                         Logger.LogInformation(logMessage);
                                                                         break;
                                                                 }
                                                             };
    
    public static void PreWarm(this IApplicationBuilder applicationBuilder)
    {
        TypeProvider.LoadDomainEventsTypeDynamically();

        IConfigurationSection subscriptionConfigSection = Startup.Configuration.GetSection("AppSettings:SubscriptionConfiguration");
        SubscriptionWorkersRoot subscriptionWorkersRoot = new();
        subscriptionConfigSection.Bind(subscriptionWorkersRoot);
        
        IDomainEventHandlerResolver mainEventHandlerResolver = Startup.Container.GetInstance<IDomainEventHandlerResolver>("Main");
        
        Dictionary<String, IDomainEventHandlerResolver> eventHandlerResolvers = new() {
                                                                                    {"Main", mainEventHandlerResolver}
                                                                                };

        Func<String, Int32, ISubscriptionRepository> subscriptionRepositoryResolver = Startup.Container.GetInstance<Func<String, Int32, ISubscriptionRepository>>();


        String connectionString = Startup.Configuration.GetValue<String>("EventStoreSettings:ConnectionString");

        applicationBuilder.ConfigureSubscriptionService(subscriptionWorkersRoot,
            connectionString,
                                                        eventHandlerResolvers,
                                                        Extensions.log,
                                                        subscriptionRepositoryResolver).Wait(CancellationToken.None);
    }
}