using MessagingService.IntegrationTests.Common;
using SecurityService.DataTransferObjects.Requests;
using SecurityService.DataTransferObjects.Responses;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingService.IntegrationTests.Shared
{
    using global::Shared.IntegrationTesting;
    using Reqnroll;
    using SecurityService.DataTransferObjects;
    using SecurityService.IntegrationTesting.Helpers;
    using ClientDetails = Common.ClientDetails;

    [Binding]
    [Scope(Tag = "shared")]
    public class SharedSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        private readonly SecurityServiceSteps SecurityServiceSteps;

        public SharedSteps(ScenarioContext scenarioContext,
                           TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
            this.SecurityServiceSteps = new SecurityServiceSteps(testingContext.DockerHelper.SecurityServiceClient);
        }

        [Given(@"I create the following api scopes")]
        public async Task GivenICreateTheFollowingApiScopes(Table table)
        {
            List<CreateApiScopeRequest> requests = table.Rows.ToCreateApiScopeRequests();
            await this.SecurityServiceSteps.GivenICreateTheFollowingApiScopes(requests);
        }

        [Given(@"the following api resources exist")]
        public async Task GivenTheFollowingApiResourcesExist(Table table)
        {
            List<CreateApiResourceRequest> requests = table.Rows.ToCreateApiResourceRequests();
            await this.SecurityServiceSteps.GivenTheFollowingApiResourcesExist(requests);
        }

        [Given(@"the following clients exist")]
        public async Task GivenTheFollowingClientsExist(Table table)
        {
            List<CreateClientRequest> requests = table.Rows.ToCreateClientRequests();
            List<(String clientId, String secret, List<String> allowedGrantTypes)> clients = await this.SecurityServiceSteps.GivenTheFollowingClientsExist(requests);
            foreach ((String clientId, String secret, List<String> allowedGrantTypes) client in clients)
            {
                this.TestingContext.AddClientDetails(client.clientId, client.secret, String.Join(",", client.allowedGrantTypes));
            }
        }

        [Given(@"I have a token to access the messaging service resource")]
        public async Task GivenIHaveATokenToAccessTheMessagingServiceResource(DataTable table)
        {
            DataTableRow firstRow = table.Rows.First();
            String clientId = ReqnrollTableHelper.GetStringRowValue(firstRow, "ClientId");
            ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

            this.TestingContext.AccessToken = await this.SecurityServiceSteps.GetClientToken(clientDetails.ClientId, clientDetails.ClientSecret, CancellationToken.None);
        }

    }
}
