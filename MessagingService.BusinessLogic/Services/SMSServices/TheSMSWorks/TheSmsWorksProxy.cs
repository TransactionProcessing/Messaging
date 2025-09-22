using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Requests;
    using Shared.Exceptions;
    using Shared.Extensions;
    using Shared.General;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksProxy : ISMSServiceProxy
    {
        private readonly HttpClient HttpClient;

        public TheSmsWorksProxy(HttpClient httpClient) {
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<SMSServiceProxyResponse> SendSMS(Guid messageId,
                                                           String sender,
                                                           String destination,
                                                           String message,
                                                           CancellationToken cancellationToken) {
            SMSServiceProxyResponse response = null;

            // Create the Auth Request                
            TheSmsWorksTokenRequest apiTokenRequest = new() { CustomerId = ConfigurationReader.GetValue("TheSMSWorksCustomerId"), Key = ConfigurationReader.GetValue("TheSMSWorksKey"), Secret = ConfigurationReader.GetValue("TheSMSWorksSecret") };

            String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
            StringContent content = new(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

            // First do the authentication
            HttpResponseMessage apiTokenHttpResponse = await this.HttpClient.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}auth/token", content, cancellationToken);

            if (apiTokenHttpResponse.IsSuccessStatusCode) {
                TheSmsWorksTokenResponse apiTokenResponse = JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content.ReadAsStringAsync());

                // Now do the actual send
                TheSmsWorksSendSMSRequest apiSendSmsRequest = new() {
                    Content = message,
                    Sender = sender,
                    Destination = destination,
                    Schedule = string.Empty,
                    Tag = messageId.ToString(),
                    Ttl = 0
                };

                String apiSendSMSMessageRequestSerialised = JsonConvert.SerializeObject(apiSendSmsRequest).ToLower();
                content = new StringContent(apiSendSMSMessageRequestSerialised, Encoding.UTF8, "application/json");

                this.HttpClient.DefaultRequestHeaders.Add("Authorization", apiTokenResponse.Token);
                HttpResponseMessage apiSendSMSMessageHttpResponse = await this.HttpClient.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}message/send", content, cancellationToken);

                if (apiSendSMSMessageHttpResponse.IsSuccessStatusCode) {
                    // Message has been sent
                    TheSmsWorksSendSMSResponse apiTheSmsWorksSendSmsResponse = JsonConvert.DeserializeObject<TheSmsWorksSendSMSResponse>(await apiSendSMSMessageHttpResponse.Content.ReadAsStringAsync());

                    response = new SMSServiceProxyResponse { ApiCallSuccessful = true, SMSIdentifier = apiTheSmsWorksSendSmsResponse.MessageId, };
                }
                else {
                    response = await HandleAPIError(apiSendSMSMessageHttpResponse);
                }
            }
            else {
                response = await HandleAPIError(apiTokenHttpResponse);
            }

            return response;
        }

        public async Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                                  CancellationToken cancellationToken) {
            MessageStatusResponse response = new();

            // Create the Auth Request                
            TheSmsWorksTokenRequest apiTokenRequest = new() { CustomerId = ConfigurationReader.GetValue("TheSMSWorksCustomerId"), Key = ConfigurationReader.GetValue("TheSMSWorksKey"), Secret = ConfigurationReader.GetValue("TheSMSWorksSecret") };

            String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
            StringContent content = new(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

            // First do the authentication
            HttpResponseMessage apiTokenHttpResponse = await this.HttpClient.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}auth/token", content, cancellationToken);

            if (apiTokenHttpResponse.IsSuccessStatusCode) {
                TheSmsWorksTokenResponse apiTokenResponse = JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content.ReadAsStringAsync());

                this.HttpClient.DefaultRequestHeaders.Add("Authorization", apiTokenResponse.Token);
                HttpResponseMessage apiGetSMSMessageHttpResponse = await this.HttpClient.GetAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}messages/{providerReference}", cancellationToken);

                if (apiGetSMSMessageHttpResponse.IsSuccessStatusCode) {
                    // Message has been sent
                    TheSMSWorksGetMessageResponse apiSmsWorksGetMessageResponse = JsonConvert.DeserializeObject<TheSMSWorksGetMessageResponse>(await apiGetSMSMessageHttpResponse.Content.ReadAsStringAsync());

                    response = new MessageStatusResponse { ApiStatusCode = apiGetSMSMessageHttpResponse.StatusCode, MessageStatus = this.TranslateMessageStatus(apiSmsWorksGetMessageResponse.Status), ProviderStatusDescription = apiSmsWorksGetMessageResponse.Status, Timestamp = DateTime.Parse(apiSmsWorksGetMessageResponse.Modified) };
                }
                else {
                    throw new NotFoundException($"Error getting message Id [{providerReference}]");
                }
            }
            else {
                throw new Exception("Authentication Error");
            }

            return response;
        }

        /// <summary>
        /// Translates the message status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        private MessageStatus TranslateMessageStatus(String status)
        {
            MessageStatus result = MessageStatus.NotSet;
            switch (status)
            {
                case "UNDELIVERABLE":
                    result = MessageStatus.Undeliverable;
                    break;
                case "DELIVERED":
                case "SENT":
                    result = MessageStatus.Delivered;
                    break;
                case "REJECTED":
                    result = MessageStatus.Rejected;
                    break;
                case "EXPIRED":
                    result = MessageStatus.Expired;
                    break;
                case "INCOMING":
                    result = MessageStatus.Incoming;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Handles the API error.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <returns></returns>
        private async Task<SMSServiceProxyResponse> HandleAPIError(HttpResponseMessage httpResponse)
        {
            SMSServiceProxyResponse response = new();

            String responseContent = await httpResponse.Content.ReadAsStringAsync();

            Boolean isValidObject = responseContent.TryParseJson(out TheSmsWorksExtendedErrorModel errorModel);

            if (isValidObject)
            {
                response.Error = errorModel.Errors.Single().Message;
                response.ErrorCode = errorModel.Errors.Single().Code;
            }

            return response;
        }

    }
}