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
    using Shared.Extensions;
    using Shared.General;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksProxy : ISMSServiceProxy
    {
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
                                                           CancellationToken cancellationToken)
        {
            SMSServiceProxyResponse response = null;

            using(HttpClient client = new HttpClient())
            {
                // Create the Auth Request                
                TheSmsWorksTokenRequest apiTokenRequest = new TheSmsWorksTokenRequest
                                                          {
                                                              CustomerId = ConfigurationReader.GetValue("TheSMSWorksCustomerId"),
                                                              Key = ConfigurationReader.GetValue("TheSMSWorksKey"),
                                                              Secret = ConfigurationReader.GetValue("TheSMSWorksSecret")
                                                          };

                String apiTokenRequestSerialised = JsonConvert.SerializeObject(apiTokenRequest).ToLower();
                StringContent content = new StringContent(apiTokenRequestSerialised, Encoding.UTF8, "application/json");

                // First do the authentication
                HttpResponseMessage apiTokenHttpResponse = await client.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}auth/token", content, cancellationToken);

                if (apiTokenHttpResponse.IsSuccessStatusCode)
                {
                    TheSmsWorksTokenResponse apiTokenResponse =
                        JsonConvert.DeserializeObject<TheSmsWorksTokenResponse>(await apiTokenHttpResponse.Content.ReadAsStringAsync());

                    // Now do the actual send
                    TheSmsWorksSendSMSRequest apiSendSmsRequest = new TheSmsWorksSendSMSRequest
                                                                  {
                                                                      Content = message,
                                                                      Sender = sender,
                                                                      Destination = destination,
                                                                      Schedule = string.Empty,
                                                                      Tag = messageId.ToString(),
                                                                      Ttl = 0
                                                                  };

                    String apiSendSMSMessageRequestSerialised = JsonConvert.SerializeObject(apiSendSmsRequest).ToLower();
                    content = new StringContent(apiSendSMSMessageRequestSerialised, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("Authorization", apiTokenResponse.Token);
                    HttpResponseMessage apiSendSMSMessageHttpResponse =
                        await client.PostAsync($"{ConfigurationReader.GetValue("TheSMSWorksBaseAddress")}message/send", content, cancellationToken);

                    if (apiSendSMSMessageHttpResponse.IsSuccessStatusCode)
                    {
                        // Message has been sent
                        TheSmsWorksSendSMSResponse apiTheSmsWorksSendSmsResponse =
                            JsonConvert.DeserializeObject<TheSmsWorksSendSMSResponse>(await apiSendSMSMessageHttpResponse.Content.ReadAsStringAsync());

                        response = new SMSServiceProxyResponse
                        {
                                       ApiStatusCode = apiSendSMSMessageHttpResponse.StatusCode,
                                       SMSIdentifier = apiTheSmsWorksSendSmsResponse.MessageId
                                   };
                    }
                    else
                    {
                        response = await HandleAPIError(apiSendSMSMessageHttpResponse);
                    }
                }
                else
                {
                    response = await HandleAPIError(apiTokenHttpResponse);
                }

            }

            return response;
        }
        
        /// <summary>
        /// Handles the API error.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <returns></returns>
        private async Task<SMSServiceProxyResponse> HandleAPIError(HttpResponseMessage httpResponse)
        {
            SMSServiceProxyResponse response = new SMSServiceProxyResponse();

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