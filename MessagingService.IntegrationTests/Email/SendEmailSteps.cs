using System;
using TechTalk.SpecFlow;

namespace MessagingService.IntegrationTests.Email
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using global::Shared.IntegrationTesting;
    using Newtonsoft.Json;
    using Shouldly;
    using SpecflowTableHelper = Common.SpecflowTableHelper;

    [Binding]
    [Scope(Tag = "email")]
    public class SendEmailSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        public SendEmailSteps(ScenarioContext scenarioContext,
                              TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
        }

        [Given(@"I send the following Email Messages")]
        public async Task GivenISendTheFollowingEmailMessages(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                await this.SendEmail(tableRow);
            }
        }

        [When(@"I resend the following messages")]
        public async Task WhenIResendTheFollowingMessages(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                await this.ResendEmail(tableRow);
            }
        }


        private async Task SendEmail(TableRow tableRow)
        {
            String fromAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "FromAddress");
            String toAddresses = SpecflowTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            String subject = SpecflowTableHelper.GetStringRowValue(tableRow, "Subject");
            String body = SpecflowTableHelper.GetStringRowValue(tableRow, "Body");
            Boolean isHtml = SpecflowTableHelper.GetBooleanValue(tableRow, "IsHtml");

            SendEmailRequest request = new SendEmailRequest
                                       {
                                           Body = body,
                                           ConnectionIdentifier = Guid.NewGuid(),
                                           FromAddress = fromAddress,
                                           IsHtml = isHtml,
                                           Subject = subject,
                                           ToAddresses = toAddresses.Split(",").ToList()
                                       };
            
            SendEmailResponse sendEmailResponse = await this.TestingContext.DockerHelper.MessagingServiceClient.SendEmail(this.TestingContext.AccessToken, request, CancellationToken.None).ConfigureAwait(false);

            sendEmailResponse.MessageId.ShouldNotBe(Guid.Empty);

            this.TestingContext.AddEmailResponse(toAddresses, sendEmailResponse);
        }

        private async Task ResendEmail(TableRow tableRow)
        {
            String toAddresses = SpecflowTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            SendEmailResponse sendEmailResponse = this.TestingContext.GetEmailResponse(toAddresses);
            
            ResendEmailRequest request = new ResendEmailRequest()
                                       {
                                           ConnectionIdentifier = Guid.NewGuid(),
                                           MessageId = sendEmailResponse.MessageId
                                       };

            await Retry.For(async () => {
                          Should.NotThrow(async () => {
                                              await this.TestingContext.DockerHelper.MessagingServiceClient.ResendEmail(this.TestingContext.AccessToken,
                                                  request,
                                                  CancellationToken.None);
                                          });
                      });
        }
    }
}
