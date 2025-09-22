namespace MessagingService.Bootstrapper;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Security;
using BusinessLogic.Common;
using Common;
using EmailMessageAggregate;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.DomainDrivenDesign.EventSourcing;
using Shared.EntityFramework.ConnectionStringConfiguration;
using Shared.EventStore.Aggregate;
using Shared.EventStore.EventStore;
using Shared.EventStore.SubscriptionWorker;
using Shared.General;
using Shared.Repositories;
using SMSMessageAggregate;

[ExcludeFromCodeCoverage]
public class RepositoryRegistry: ServiceRegistry
{
    public RepositoryRegistry()
    {
        String connectionString = Startup.Configuration.GetValue<String>("EventStoreSettings:ConnectionString");

        this.AddEventStoreProjectionManagementClient(connectionString);
        this.AddEventStorePersistentSubscriptionsClient(connectionString);

        this.AddEventStoreClient(connectionString);

        this.AddSingleton<IConnectionStringConfigurationRepository, ConfigurationReaderConnectionStringRepository>();

        this.AddTransient<IEventStoreContext, EventStoreContext>();

        this.AddSingleton<IAggregateRepository<EmailAggregate, DomainEvent>, AggregateRepository<EmailAggregate, DomainEvent>>();
        this.AddSingleton<IAggregateRepository<SMSAggregate, DomainEvent>, AggregateRepository<SMSAggregate, DomainEvent>>();

        this.AddSingleton<Func<String, Int32, ISubscriptionRepository>>(cont => (esConnString, cacheDuration) => SubscriptionRepository.Create(esConnString, cacheDuration));
    }
}