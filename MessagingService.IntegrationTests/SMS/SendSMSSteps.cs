using System;
using TechTalk.SpecFlow;

namespace MessagingService.IntegrationTests.SMS
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using Shouldly;
    
    [Binding]
    [Scope(Tag = "sms")]
    public class SendSMSSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        public SendSMSSteps(ScenarioContext scenarioContext,
                            TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
        }

        [Given(@"I send the following SMS Messages")]
        public async Task GivenISendTheFollowingSMSMessages(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                await this.SendSMS(tableRow);
            }
        }

        private async Task SendSMS(TableRow tableRow)
        {
            String sender = SpecflowTableHelper.GetStringRowValue(tableRow, "Sender");
            String destination = SpecflowTableHelper.GetStringRowValue(tableRow, "Destination");
            String message = SpecflowTableHelper.GetStringRowValue(tableRow, "Message");

            SendSMSRequest request = new SendSMSRequest
            {
                ConnectionIdentifier = Guid.NewGuid(),
                Sender = sender,
                Destination = destination,
                Message = message
            };

            SendSMSResponse sendEmailResponse = await this.TestingContext.DockerHelper.MessagingServiceClient.SendSMS(this.TestingContext.AccessToken, request, CancellationToken.None).ConfigureAwait(false);

            sendEmailResponse.MessageId.ShouldNotBe(Guid.Empty);
        }
    }
}
