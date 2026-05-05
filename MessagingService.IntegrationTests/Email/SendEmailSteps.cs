using System;

namespace MessagingService.IntegrationTests.Email
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using IntegrationTesting.Helpers;
    using Reqnroll;

    [Binding]
    [Scope(Tag = "email")]
    public class SendEmailSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        private readonly MessagingSteps MessagingSteps;

        public SendEmailSteps(ScenarioContext scenarioContext,
                              TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
            this.MessagingSteps = new MessagingSteps(this.TestingContext.DockerHelper.MessagingServiceClient);
        }

        [Given(@"I send the following Email Messages")]
        public async Task GivenISendTheFollowingEmailMessages(Table table){
            List<SendEmailRequest> requests = table.Rows.ToSendEmailRequests();
            List<(String, SendEmailResponse)> results = await this.MessagingSteps.GivenISendTheFollowingEmailMessages(this.TestingContext.AccessToken, requests);
            foreach ((String, SendEmailResponse) result in results){
                this.TestingContext.AddEmailResponse(result.Item1, result.Item2);
            }
        }

        [When(@"I resend the following messages")]
        public async Task WhenIResendTheFollowingMessages(Table table)
        {
            List<ResendEmailRequest> requests = table.Rows.ToResendEmailRequests(this.TestingContext.EmailResponses);
            await this.MessagingSteps.WhenIResendTheFollowingMessages(this.TestingContext.AccessToken, requests);
        }

    }
}
