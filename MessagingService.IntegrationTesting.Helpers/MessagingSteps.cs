namespace MessagingService.IntegrationTesting.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MessagingService.Client;
    using MessagingService.DataTransferObjects;
    using Shared.IntegrationTesting;
    using Shouldly;

    public class MessagingSteps{
        private readonly IMessagingServiceClient MessagingServiceClient;

        public MessagingSteps(IMessagingServiceClient messagingServiceClient){
            this.MessagingServiceClient = messagingServiceClient;
        }

        public async Task<List<(String, SendEmailResponse)>> GivenISendTheFollowingEmailMessages(String accessToken, List<SendEmailRequest> requests){
            List<(String, SendEmailResponse)> results = new List<(String, SendEmailResponse)>();
            foreach (SendEmailRequest sendEmailRequest in requests){
                var result = await this.MessagingServiceClient.SendEmail(accessToken, sendEmailRequest, CancellationToken.None).ConfigureAwait(false);

                result.IsSuccess.ShouldBeTrue();
                results.Add((String.Join(",", sendEmailRequest.ToAddresses),  null));
            }
            return results;
        }

        public async Task GivenISendTheFollowingSMSMessages(String accessToken, List<SendSMSRequest> requests)
        {
            foreach (SendSMSRequest sendSmsRequest in requests)
            {
                var result = await this.MessagingServiceClient.SendSMS(accessToken, sendSmsRequest, CancellationToken.None).ConfigureAwait(false);

                result.IsSuccess.ShouldBeTrue();
            }
        }

        public async Task WhenIResendTheFollowingMessages(String accessToken, List<ResendEmailRequest> requests){
            foreach (ResendEmailRequest resendEmailRequest in requests){
                await Retry.For(async () => {
                                    Should.NotThrow(async () => {
                                                        await this.MessagingServiceClient.ResendEmail(accessToken,
                                                                                                      resendEmailRequest,
                                                                                                      CancellationToken.None);
                                                    });
                                });
            }
        }

    }
}
