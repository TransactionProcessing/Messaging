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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ClientProxyBase.ClientProxyBase" />
    /// <seealso cref="MessagingService.Client.IMessagingServiceClient" />
    public class MessagingServiceClient : ClientProxyBase, IMessagingServiceClient
    {
        #region Fields

        /// <summary>
        /// The base address
        /// </summary>
        private readonly String BaseAddress;

        /// <summary>
        /// The base address resolver
        /// </summary>
        private readonly Func<String, String> BaseAddressResolver;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingServiceClient"/> class.
        /// </summary>
        /// <param name="baseAddressResolver">The base address resolver.</param>
        /// <param name="httpClient">The HTTP client.</param>
        public MessagingServiceClient(Func<String, String> baseAddressResolver,
                                      HttpClient httpClient) : base(httpClient)
        {
            this.BaseAddressResolver = baseAddressResolver;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="sendEmailRequest">The send email request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendEmailResponse> SendEmail(String accessToken,
                                                       SendEmailRequest sendEmailRequest,
                                                       CancellationToken cancellationToken)
        {
            SendEmailResponse response = null;

            String requestUri = this.BuildRequestUrl("/api/email/");

            try
            {
                String requestSerialised = JsonConvert.SerializeObject(sendEmailRequest);

                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

                // Add the access token to the client headers
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.PostAsync(requestUri, httpContent, cancellationToken);

                // Process the response
                String content = await this.HandleResponse(httpResponse, cancellationToken);

                // call was successful so now deserialise the body to the response object
                response = JsonConvert.DeserializeObject<SendEmailResponse>(content);
            }
            catch(Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception("Error sending email message.", ex);

                throw exception;
            }

            return response;
        }

        public async Task ResendEmail(String accessToken,
                                ResendEmailRequest resendEmailRequest,
                                CancellationToken cancellationToken) {

            String requestUri = this.BuildRequestUrl("/api/email/resend");

            try {
                String requestSerialised = JsonConvert.SerializeObject(resendEmailRequest);

                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

                // Add the access token to the client headers
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.PostAsync(requestUri, httpContent, cancellationToken);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch(Exception ex) {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception("Error re-sending email message.", ex);

                throw exception;
            }
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="sendSMSRequest">The send SMS request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SendSMSResponse> SendSMS(String accessToken,
                                                   SendSMSRequest sendSMSRequest,
                                                   CancellationToken cancellationToken)
        {
            SendSMSResponse response = null;

            String requestUri = this.BuildRequestUrl("/api/sms/");

            try
            {
                String requestSerialised = JsonConvert.SerializeObject(sendSMSRequest);

                StringContent httpContent = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

                // Add the access token to the client headers
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.PostAsync(requestUri, httpContent, cancellationToken);

                // Process the response
                String content = await this.HandleResponse(httpResponse, cancellationToken);

                // call was successful so now deserialise the body to the response object
                response = JsonConvert.DeserializeObject<SendSMSResponse>(content);
            }
            catch (Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception("Error sending sms message.", ex);

                throw exception;
            }

            return response;
        }

        /// <summary>
        /// Builds the request URL.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        private String BuildRequestUrl(String route)
        {
            String baseAddress = this.BaseAddressResolver("MessagingServiceApi");

            String requestUri = $"{baseAddress}{route}";

            return requestUri;
        }

        #endregion
    }
}