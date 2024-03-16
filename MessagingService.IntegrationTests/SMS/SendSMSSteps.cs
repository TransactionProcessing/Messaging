using System;

namespace MessagingService.IntegrationTests.SMS
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using IntegrationTesting.Helpers;
    using Reqnroll;
    using Shared;
    using Shouldly;
    
    [Binding]
    [Scope(Tag = "sms")]
    public class SendSMSSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        private readonly MessagingSteps MessagingSteps;
        public SendSMSSteps(ScenarioContext scenarioContext,
                            TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
            this.MessagingSteps = new MessagingSteps(testingContext.DockerHelper.MessagingServiceClient);
        }

        [Given(@"I send the following SMS Messages")]
        public async Task GivenISendTheFollowingSMSMessages(Table table){
            List<SendSMSRequest> requests = table.Rows.ToSendSMSRequests();
            await this.MessagingSteps.GivenISendTheFollowingSMSMessages(this.TestingContext.AccessToken, requests);
        }
    }
}
