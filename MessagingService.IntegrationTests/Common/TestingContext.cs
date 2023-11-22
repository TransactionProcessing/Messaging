using Shared.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.IntegrationTests.Common
{
    using System.Linq;
    using DataTransferObjects;
    using SecurityService.DataTransferObjects.Responses;
    using Shouldly;

    public class TestingContext
    {
        public TestingContext()
        {
            this.Clients = new List<ClientDetails>();
            this.EmailResponses=new Dictionary<String, SendEmailResponse>();
        }

        public NlogLogger Logger { get; set; }
        public String AccessToken { get; set; }
        public DockerHelper DockerHelper { get; set; }

        private List<ClientDetails> Clients;
        public void AddClientDetails(String clientId,
                                     String clientSecret,
                                     String grantType)
        {
            this.Clients.Add(ClientDetails.Create(clientId, clientSecret, grantType));
        }

        public ClientDetails GetClientDetails(String clientId)
        {
            ClientDetails clientDetails = this.Clients.SingleOrDefault(c => c.ClientId == clientId);

            clientDetails.ShouldNotBeNull();

            return clientDetails;
        }

        public void AddEmailResponse(String identifier, SendEmailResponse response) {
            this.EmailResponses.Add(identifier, response);
        }

        public SendEmailResponse GetEmailResponse(String identifier) {
            return this.EmailResponses[identifier];
        }

        public Dictionary<String, SendEmailResponse> EmailResponses;
    }

    public class ClientDetails
    {
        public String ClientId { get; private set; }
        public String ClientSecret { get; private set; }
        public String GrantType { get; private set; }

        private ClientDetails(String clientId,
                              String clientSecret,
                              String grantType)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.GrantType = grantType;
        }

        public static ClientDetails Create(String clientId,
                                           String clientSecret,
                                           String grantType)
        {
            return new ClientDetails(clientId, clientSecret, grantType);
        }
    }
}
