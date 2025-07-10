using ClientProxyBase;

namespace MessagingService.IntegrationTests.Common
{
    using Client;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Common;
    using Ductus.FluentDocker.Model.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using global::Shared.IntegrationTesting;
    using global::Shared.Logger;
    using IntegrationTesting.Helpers;
    using Microsoft.AspNetCore.Http;
    using SecurityService.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    
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
        public DockerHelper()
        {
            this.TestingContext = new TestingContext();     
            this.TestId = Guid.NewGuid();
        }

        #endregion

        #region Methods

        public override async Task CreateSubscriptions(){
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = SubscriptionsHelper.GetSubscriptions();
            foreach ((String streamName, String groupName, Int32 maxRetries) subscription in subscriptions)
            {
                await this.CreatePersistentSubscription(subscription);
            }
        }

        /// <summary>
        /// Starts the containers for scenario run.
        /// </summary>
        /// <param name="scenarioName">Name of the scenario.</param>
        public override async Task StartContainersForScenarioRun(String scenarioName, DockerServices dockerServices){
            await base.StartContainersForScenarioRun(scenarioName, dockerServices);

            // Setup the base address resolvers
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String MessagingServiceBaseAddressResolver(String api) => $"http://127.0.0.1:{this.MessagingServicePort}";
            
            HttpClient httpClient = CreateHttpClient();
            
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient);
            this.MessagingServiceClient = new MessagingServiceClient(MessagingServiceBaseAddressResolver, httpClient);
        }
        #endregion

        private HttpClient CreateHttpClient() {
            // Set up test HttpContext
            DefaultHttpContext context = new DefaultHttpContext();
            context.TraceIdentifier = this.TestId.ToString();

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = context
            };

            // Configure inner-most handler with SSL bypass
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // Wrap with CorrelationIdHandler
            CorrelationIdHandler correlationHandler = new CorrelationIdHandler(httpContextAccessor)
            {
                InnerHandler = clientHandler
            };

            // Create HttpClient
            HttpClient client = new HttpClient(correlationHandler);

            return client;
        }
    }
}
