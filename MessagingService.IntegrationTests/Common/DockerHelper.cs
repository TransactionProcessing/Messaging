using TransactionProcessing.ClientProxyBase;
using TransactionProcessing.UnionResults;

namespace MessagingService.IntegrationTests.Common
{
    using Client;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Common;
    using Ductus.FluentDocker.Model.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using DotNet.Testcontainers.Configurations;
    using global::TransactionProcessing.Shared.IntegrationTesting;
    using global::TransactionProcessing.Shared.Logger;
    using global::TransactionProcessing.Shared.Serialisation;
    using IntegrationTesting.Helpers;
    using Microsoft.AspNetCore.Http;
    using SecurityService.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessing.Shared.IntegrationTesting.DockerHelper" />
    public class DockerHelper : global::TransactionProcessing.Shared.IntegrationTesting.TestContainers.DockerHelper
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

        public override async Task CreateSubscriptions(CancellationToken cancellationToken){
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = SubscriptionsHelper.GetSubscriptions();
            foreach ((String streamName, String groupName, Int32 maxRetries) subscription in subscriptions)
            {
                await this.CreatePersistentSubscription(subscription);
            }
        }

        public String AccessToken;

        public override async Task StartContainersForScenarioRun(String scenarioName, DockerServices dockerServices, CancellationToken cancellationToken)
        {
            await base.StartContainersForScenarioRun(scenarioName, dockerServices, cancellationToken);

            // Setup the base address resolvers
            String SecurityServiceBaseAddressResolver(String api) => $"https://127.0.0.1:{this.SecurityServicePort}";
            String MessagingServiceBaseAddressResolver(String api) => $"https://localhost:{this.MessagingServicePort}";
            
            HttpClient httpClient = CreateHttpClient();

            using HttpResponseMessage messagingHealthResponse = await httpClient.GetAsync(
                $"https://localhost:{this.MessagingServicePort}/health",
                cancellationToken).ConfigureAwait(false);
            messagingHealthResponse.EnsureSuccessStatusCode();
            
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient, Serialise, Deserialise);
            this.MessagingServiceClient = new MessagingServiceClient(MessagingServiceBaseAddressResolver, httpClient, Serialise, Deserialise);

            var bootstrapToken = await this.SecurityServiceClient.GetToken("management-bootstrap", "management-bootstrap-secret", CancellationToken.None);
            if (bootstrapToken.Value is not Success<SecurityService.DataTransferObjects.TokenResponse> success ||
                String.IsNullOrWhiteSpace(success.Value?.AccessToken))
            {
                throw new InvalidOperationException("Unable to obtain the integration-test management bootstrap token.");
            }
            this.AccessToken = success.Value.AccessToken;
        }

        protected override IReadOnlyList<IReadOnlyList<DockerServices>> GetStartupGroups()
        {
            List<IReadOnlyList<DockerServices>> startupGroups = new();

            foreach (IReadOnlyList<DockerServices> group in base.GetStartupGroups())
            {
                if (group.Contains(DockerServices.MessagingService))
                {
                    startupGroups.Add(new[] { DockerServices.MessagingService });

                    DockerServices[] remainingServices = group
                        .Where(service => service != DockerServices.MessagingService)
                        .ToArray();

                    if (remainingServices.Length > 0)
                    {
                        startupGroups.Add(remainingServices);
                    }
                }
                else
                {
                    startupGroups.Add(group);
                }
            }

            return startupGroups;
        }

        public override DotNet.Testcontainers.Builders.ContainerBuilder SetupMessagingServiceContainer()
        {
            if (this.IntegrationTestCertificate is null)
            {
                throw new InvalidOperationException("Integration test certificate was not initialised before MessagingService container setup.");
            }

            this.SkipHealthChecks = true;
            this.AdditionalVariables[ContainerType.MessagingService] = new Dictionary<String, String>
            {
                ["ServiceOptions:KestrelOptions:Path"] = this.IntegrationTestCertificate.GetContainerCertificatePath(),
                ["ServiceOptions:KestrelOptions:Password"] = this.IntegrationTestCertificate.Password
            };

            return base.SetupMessagingServiceContainer()
                .WithResourceMapping(
                    this.IntegrationTestCertificate.CertificateDirectory,
                    this.IntegrationTestCertificate.ContainerCertificateDirectory);
        }

        public override DotNet.Testcontainers.Builders.ContainerBuilder SetupSecurityServiceContainer()
        {
            this.SkipHealthChecks = false;
            return base.SetupSecurityServiceContainer();
        }
        #endregion

        String Serialise(Object arg)
        {
            return StringSerialiser.Serialise<Object>(arg, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        Object Deserialise(String arg, Type type)
        {
            return StringSerialiser.DeserializeObject<Object>(arg, type, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        }

        private HttpClient CreateHttpClient() {
            // Set up test HttpContext
            DefaultHttpContext context = new();
            context.TraceIdentifier = this.TestId.ToString();

            HttpContextAccessor httpContextAccessor = new()
            {
                HttpContext = context
            };

            // Configure inner-most handler with SSL bypass
            HttpClientHandler clientHandler = new()
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
