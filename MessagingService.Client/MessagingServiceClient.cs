using Shared.Results;

namespace MessagingService.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientProxyBase;
    using DataTransferObjects;
    using Newtonsoft.Json;
    using SimpleResults;

    public class MessagingServiceClient : ClientProxyBase, IMessagingServiceClient
    {
        #region Fields

        private readonly String BaseAddress;

        private readonly Func<String, String> BaseAddressResolver;

        #endregion

        #region Constructors

        public MessagingServiceClient(Func<String, String> baseAddressResolver,
                                      HttpClient httpClient) : base(httpClient)
        {
            this.BaseAddressResolver = baseAddressResolver;
        }

        #endregion

        #region Methods

        public async Task<Result> SendEmail(String accessToken,
                                                       SendEmailRequest sendEmailRequest,
                                                       CancellationToken cancellationToken)
        {
            String requestUri = this.BuildRequestUrl("/api/email/");

            try
            {
                Result<String> result = await this.SendPostRequest<SendEmailRequest, String>(requestUri, accessToken, sendEmailRequest, cancellationToken);
                
                if (result.IsFailed)
                    return ResultHelpers.CreateFailure(result);

                return Result.Success();
            }
            catch(Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new("Error sending email message.", ex);

                throw exception;
            }
        }

        public async Task<Result> ResendEmail(String accessToken,
                                              ResendEmailRequest resendEmailRequest,
                                              CancellationToken cancellationToken) {
            String requestUri = this.BuildRequestUrl("/api/email/resend");

            try {
                Result<String> result = await this.SendPostRequest<ResendEmailRequest, String>(requestUri, accessToken, resendEmailRequest, cancellationToken);

                if (result.IsFailed)
                    return ResultHelpers.CreateFailure(result);

                return Result.Success();
            }
            catch(Exception ex) {
                // An exception has occurred, add some additional information to the message
                Exception exception = new("Error re-sending email message.", ex);

                throw exception;
            }
        }

        public async Task<Result> SendSMS(String accessToken,
                                          SendSMSRequest sendSMSRequest,
                                          CancellationToken cancellationToken)
        {
            String requestUri = this.BuildRequestUrl("/api/sms/");

            try
            {
                Result<String> result = await this.SendPostRequest<SendSMSRequest, String>(requestUri, accessToken, sendSMSRequest, cancellationToken);

                if (result.IsFailed)
                    return ResultHelpers.CreateFailure(result);

                return Result.Success();
            }
            catch (Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new("Error sending sms message.", ex);

                throw exception;
            }
        }
        private String BuildRequestUrl(String route)
        {
            String baseAddress = this.BaseAddressResolver("MessagingServiceApi");

            String requestUri = $"{baseAddress}{route}";

            return requestUri;
        }

        #endregion
    }
}