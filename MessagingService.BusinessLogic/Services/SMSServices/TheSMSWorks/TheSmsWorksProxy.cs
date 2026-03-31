using System;
using System.Collections.Generic;
using System.Security.Authentication;
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

    public record SmsWorksConfig {
        public String BaseAddress { get; set; }
        public String CustomerId { get; set; }
        public String Key { get; set; }
        public String Secret { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksProxy : ISMSServiceProxy
    {
        private readonly HttpClient HttpClient;
        private readonly SmsWorksConfig Configuration;

        public TheSmsWorksProxy(HttpClient httpClient, SmsWorksConfig configuration) {
            this.HttpClient = httpClient;
            this.Configuration = configuration;
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
            TheSmsWorksTokenRequest apiTokenRequest = new() { CustomerId = Configuration.CustomerId, Key = Configuration.Key, Secret = Configuration.Secret };

            String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
            StringContent content = new(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

            // First do the authentication
            HttpResponseMessage apiTokenHttpResponse = await this.HttpClient.PostAsync($"{Configuration.BaseAddress}auth/token", content, cancellationToken);

            if (apiTokenHttpResponse.IsSuccessStatusCode) {
                TheSmsWorksTokenResponse apiTokenResponse = JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content.ReadAsStringAsync(cancellationToken));

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
                HttpResponseMessage apiSendSMSMessageHttpResponse = await this.HttpClient.PostAsync($"{Configuration.BaseAddress}message/send", content, cancellationToken);

                if (apiSendSMSMessageHttpResponse.IsSuccessStatusCode) {
                    // Message has been sent
                    TheSmsWorksSendSMSResponse apiTheSmsWorksSendSmsResponse = JsonConvert.DeserializeObject<TheSmsWorksSendSMSResponse>(await apiSendSMSMessageHttpResponse.Content.ReadAsStringAsync(cancellationToken));

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
            TheSmsWorksTokenRequest apiTokenRequest = new() { CustomerId = Configuration.CustomerId, Key = Configuration.Key, Secret = Configuration.Secret };

            String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
            StringContent content = new(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

            // First do the authentication
            HttpResponseMessage apiTokenHttpResponse = await this.HttpClient.PostAsync($"{Configuration.BaseAddress}auth/token", content, cancellationToken);

            if (apiTokenHttpResponse.IsSuccessStatusCode) {
                TheSmsWorksTokenResponse apiTokenResponse = JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content.ReadAsStringAsync(cancellationToken));

                this.HttpClient.DefaultRequestHeaders.Add("Authorization", apiTokenResponse.Token);
                HttpResponseMessage apiGetSMSMessageHttpResponse = await this.HttpClient.GetAsync($"{Configuration.BaseAddress}messages/{providerReference}", cancellationToken);

                if (apiGetSMSMessageHttpResponse.IsSuccessStatusCode) {
                    // Message has been sent
                    TheSMSWorksGetMessageResponse apiSmsWorksGetMessageResponse = JsonConvert.DeserializeObject<TheSMSWorksGetMessageResponse>(await apiGetSMSMessageHttpResponse.Content.ReadAsStringAsync(cancellationToken));

                    response = new MessageStatusResponse { ApiStatusCode = apiGetSMSMessageHttpResponse.StatusCode, MessageStatus = this.TranslateMessageStatus(apiSmsWorksGetMessageResponse.Status), ProviderStatusDescription = apiSmsWorksGetMessageResponse.Status, Timestamp = DateTime.Parse(apiSmsWorksGetMessageResponse.Modified) };
                }
                else {
                    throw new NotFoundException($"Error getting message Id [{providerReference}]");
                }
            }
            else {
                throw new AuthenticationException("Authentication Error");
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
            return status switch {
                "UNDELIVERABLE" => MessageStatus.Undeliverable,
                "DELIVERED"=> MessageStatus.Delivered,
                "SENT" => MessageStatus.Delivered,
                "REJECTED" => MessageStatus.Rejected,
                "EXPIRED" => MessageStatus.Expired,
                "INCOMING" => MessageStatus.Incoming,
                _ => MessageStatus.Rejected
            };
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