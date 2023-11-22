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
        Boolean useConnectionStringConfig = Boolean.Parse(ConfigurationReader.GetValue("AppSettings", "UseConnectionStringConfig"));

        if (useConnectionStringConfig)
        {
            String connectionStringConfigurationConnString = ConfigurationReader.GetConnectionString("ConnectionStringConfiguration");
            this.AddSingleton<IConnectionStringConfigurationRepository, ConnectionStringConfigurationRepository>();
            this.AddTransient<ConnectionStringConfigurationContext>(c =>
                                                                    {
                                                                        return new ConnectionStringConfigurationContext(connectionStringConfigurationConnString);
                                                                    });

            // TODO: Read this from a the database and set
        }
        else
        {
            Boolean insecureES = Startup.Configuration.GetValue<Boolean>("EventStoreSettings:Insecure");

            Func<SocketsHttpHandler> CreateHttpMessageHandler = () => new SocketsHttpHandler
                                                                      {
                                                                          SslOptions = new SslClientAuthenticationOptions
                                                                                       {
                                                                                           RemoteCertificateValidationCallback = (sender,
                                                                                               certificate,
                                                                                               chain,
                                                                                               errors) => {
                                                                                               return true;
                                                                                           }
                                                                                       }
                                                                      };

            this.AddEventStoreProjectionManagementClient(Startup.ConfigureEventStoreSettings);
            this.AddEventStorePersistentSubscriptionsClient(Startup.ConfigureEventStoreSettings);
            
            if (insecureES)
            {
                this.AddInSecureEventStoreClient(Startup.EventStoreClientSettings.ConnectivitySettings.Address, CreateHttpMessageHandler);
            }
            else
            {
                this.AddEventStoreClient(Startup.EventStoreClientSettings.ConnectivitySettings.Address, CreateHttpMessageHandler);
            }

            this.AddSingleton<IConnectionStringConfigurationRepository, ConfigurationReaderConnectionStringRepository>();
        }

        this.AddTransient<IEventStoreContext, EventStoreContext>();

        this.AddSingleton<IAggregateRepository<EmailAggregate, DomainEvent>, AggregateRepository<EmailAggregate, DomainEvent>>();
        this.AddSingleton<IAggregateRepository<SMSAggregate, DomainEvent>, AggregateRepository<SMSAggregate, DomainEvent>>();

        this.AddSingleton<Func<String, Int32, ISubscriptionRepository>>(cont => (esConnString, cacheDuration) => {
                                                                                    return SubscriptionRepository.Create(esConnString, cacheDuration);
                                                                                });
    }
}