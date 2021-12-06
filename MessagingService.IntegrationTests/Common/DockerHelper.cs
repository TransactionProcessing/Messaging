namespace MessagingService.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Client;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Common;
    using Ductus.FluentDocker.Model.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using global::Shared.Logger;
    using SecurityService.Client;
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.IntegrationTesting.DockerHelper" />
    public class DockerHelper : global::Shared.IntegrationTesting.DockerHelper
    {
        #region Fields

        /// <summary>
        /// The security service client
        /// </summary>
        public ISecurityServiceClient SecurityServiceClient;

        /// <summary>
        /// The messaging service client
        /// </summary>
        public IMessagingServiceClient MessagingServiceClient;

        /// <summary>
        /// The test identifier
        /// </summary>
        public Guid TestId;

        /// <summary>
        /// The containers
        /// </summary>
        protected List<IContainerService> Containers;
        
        /// <summary>
        /// The event store HTTP port
        /// </summary>
        protected Int32 EventStoreHttpPort;
        
        /// <summary>
        /// The security service port
        /// </summary>
        protected Int32 SecurityServicePort;

        /// <summary>
        /// The messaging service port
        /// </summary>
        protected Int32 MessagingServicePort;

        /// <summary>
        /// The test networks
        /// </summary>
        protected List<INetworkService> TestNetworks;

        /// <summary>
        /// The testing context
        /// </summary>
        private readonly TestingContext TestingContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="testingContext">The testing context.</param>
        public DockerHelper(NlogLogger logger,
                            TestingContext testingContext)
        {
            this.Logger = logger;
            this.TestingContext = testingContext;
            this.Containers = new List<IContainerService>();
            this.TestNetworks = new List<INetworkService>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName)
        {
            this.HostTraceFolder = FdOs.IsWindows() ? $"D:\\home\\txnproc\\trace\\{scenarioName}" : $"//home//txnproc//trace//{scenarioName}";

            Logging.Enabled();

            Guid testGuid = Guid.NewGuid();
            this.TestId = testGuid;

            this.Logger.LogInformation($"Test Id is {testGuid}");

            // Setup the container names
            this.SecurityServiceContainerName = $"securityservice{testGuid:N}";
            this.EventStoreContainerName = $"eventstore{testGuid:N}";
            this.MessagingServiceContainerName = $"messagingservice{testGuid:N}";
            
            this.DockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");

            INetworkService testNetwork = DockerHelper.SetupTestNetwork();
            this.TestNetworks.Add(testNetwork);

            IContainerService eventStoreContainer = 
                this.SetupEventStoreContainer("eventstore/eventstore:21.10.0-buster-slim", testNetwork, true);

            String eventStoreAddress =
                $"esdb://admin:changeit@{this.EventStoreContainerName}:{DockerHelper.EventStoreHttpDockerPort}?tls=false";

            String insecureEventStoreEnvironmentVariable = "EventStoreSettings:Insecure=true";
            String persistentSubscriptionPollingInSeconds = "AppSettings:PersistentSubscriptionPollingInSeconds=10";
            String internalSubscriptionServiceCacheDuration = "AppSettings:InternalSubscriptionServiceCacheDuration=0";

            IContainerService securityServiceContainer = this.SetupSecurityServiceContainer("stuartferguson/securityservice",
                                                                                                    testNetwork,
                                                                                                    true);

            IContainerService messagingServiceContainer = this.SetupMessagingServiceContainer("messagingservice",
                                                                                              new List<INetworkService>
                                                                                              {
                                                                                                  testNetwork
                                                                                              },
                                                                                              additionalEnvironmentVariables:new List<String>
                                                                                                  {
                                                                                                      insecureEventStoreEnvironmentVariable,
                                                                                                      internalSubscriptionServiceCacheDuration,
                                                                                                      persistentSubscriptionPollingInSeconds
                                                                                                  });

            this.Containers.AddRange(new List<IContainerService>
                                     {
                                         eventStoreContainer,
                                         securityServiceContainer,
                                         messagingServiceContainer
                                     });

            // Cache the ports
            this.SecurityServicePort = securityServiceContainer.ToHostExposedEndpoint("5001/tcp").Port;
            this.EventStoreHttpPort = eventStoreContainer.ToHostExposedEndpoint("2113/tcp").Port;
            this.MessagingServicePort = messagingServiceContainer.ToHostExposedEndpoint("5006/tcp").Port;

            // Setup the base address resolvers
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String MessagingServiceBaseAddressResolver(String api) => $"http://127.0.0.1:{this.MessagingServicePort}";

            HttpClientHandler clientHandler = new HttpClientHandler
                                              {
                                                  ServerCertificateCustomValidationCallback = (message,
                                                                                               certificate2,
                                                                                               arg3,
                                                                                               arg4) =>
                                                                                              {
                                                                                                  return true;
                                                                                              }

                                              };
            HttpClient httpClient = new HttpClient(clientHandler);
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient);
            this.MessagingServiceClient = new MessagingServiceClient(MessagingServiceBaseAddressResolver, httpClient);
        }

        /// <summary>
        /// Stops the containers for scenario run.
        /// </summary>
        public override async Task StopContainersForScenarioRun()
        {
            if (this.Containers.Any())
            {
                foreach (IContainerService containerService in this.Containers)
                {
                    containerService.StopOnDispose = true;
                    containerService.RemoveOnDispose = true;
                    containerService.Dispose();
                }
            }

            if (this.TestNetworks.Any())
            {
                foreach (INetworkService networkService in this.TestNetworks)
                {
                    networkService.Stop();
                    networkService.Remove(true);
                }
            }
        }

        #endregion
    }
}
