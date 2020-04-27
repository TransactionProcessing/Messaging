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
    using Newtonsoft.Json;
    using Shouldly;

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

            StringContent requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            this.TestingContext.DockerHelper.MessagingServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.TestingContext.AccessToken);
            
            HttpResponseMessage httpResponse = await this.TestingContext.DockerHelper.MessagingServiceClient.PostAsync("api/Email", requestContent, CancellationToken.None).ConfigureAwait(false);

            httpResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            String responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            responseContent.ShouldNotBeNullOrEmpty();
            SendEmailResponse response = JsonConvert.DeserializeObject<SendEmailResponse>(responseContent);
            response.MessageId.ShouldNotBe(Guid.Empty);
        }
    }
}
