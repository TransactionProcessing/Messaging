namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using Models;
    using Newtonsoft.Json;
    using Service.Services.Email.Smtp2Go;
    using Shared.General;
    using Shared.Logger;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.EmailServices.IEmailServiceProxy" />
    [ExcludeFromCodeCoverage]
    public class Smtp2GoProxy : IEmailServiceProxy
    {
        #region Fields

        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient HttpClient;

        #endregion

        #region Constructors

        public Smtp2GoProxy(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        #endregion

        #region Methods

        public async Task<EmailServiceProxyResponse> SendEmail(Guid messageId,
                                                               String fromAddress,
                                                               List<String> toAddresses,
                                                               String subject,
                                                               String body,
                                                               Boolean isHtml,
                                                               List<EmailAttachment> attachments,
                                                               CancellationToken cancellationToken) {
            EmailServiceProxyResponse response = null;

            // Translate the request message
            Smtp2GoSendEmailRequest apiRequest = new()
                                                 {
                                                     ApiKey = ConfigurationReader.GetValue("SMTP2GoAPIKey"),
                                                     HTMLBody = isHtml ? body : string.Empty,
                                                     TextBody = isHtml ? string.Empty : body,
                                                     Sender = fromAddress,
                                                     Subject = subject,
                                                     TestMode = false,
                                                     To = toAddresses.ToArray()
                                                 };
            if (attachments != null && attachments.Any())
            {
                apiRequest.Attachments = new List<Smtp2GoAttachment>();
                foreach (EmailAttachment emailAttachment in attachments)
                {
                    apiRequest.Attachments.Add(new Smtp2GoAttachment
                    {
                        FileBlob = emailAttachment.FileData,
                        FileName = emailAttachment.Filename,
                        MimeType = this.ConvertFileType(emailAttachment.FileType)
                    });
                }
            }

            String requestSerialised = JsonConvert.SerializeObject(apiRequest, Formatting.Indented, new JsonSerializerSettings
                                                                                                {
                                                                                                    TypeNameHandling = TypeNameHandling.None
                                                                                                });

            Logger.LogDebug($"Request Message Sent to Email Provider [SMTP2Go] {requestSerialised}");

            StringContent content = new(requestSerialised, Encoding.UTF8, "application/json");

            String requestUri = $"{ConfigurationReader.GetValue("SMTP2GoBaseAddress")}email/send";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            requestMessage.Content = content;

            HttpResponseMessage httpResponse = await this.HttpClient.SendAsync(requestMessage, cancellationToken);

            Smtp2GoSendEmailResponse apiResponse = new() { Data = new Smtp2GoSendEmailResponseData() };
            if (httpResponse.IsSuccessStatusCode){
                apiResponse = JsonConvert.DeserializeObject<Smtp2GoSendEmailResponse>(await httpResponse.Content.ReadAsStringAsync(cancellationToken));
            }
            else{
                apiResponse = new Smtp2GoSendEmailResponse();
                apiResponse.Data = new Smtp2GoSendEmailResponseData();
                apiResponse.Data.Error = httpResponse.StatusCode.ToString();
                apiResponse.Data.ErrorCode = ((Int32)httpResponse.StatusCode).ToString();
            }

            Logger.LogDebug($"Response Message Received from Email Provider [SMTP2Go] {JsonConvert.SerializeObject(apiResponse)}");

            // Translate the Response
            response = new EmailServiceProxyResponse
                       {
                           ApiCallSuccessful = httpResponse.IsSuccessStatusCode && String.IsNullOrEmpty(apiResponse.Data.ErrorCode),
                           EmailIdentifier = apiResponse.Data.EmailId,
                           Error = apiResponse.Data.Error,
                           ErrorCode = apiResponse.Data.ErrorCode,
                           RequestIdentifier = apiResponse.RequestId
                       };
            return response;
        }
        
        private String ConvertFileType(FileType emailAttachmentFileType)
        {
            switch(emailAttachmentFileType)
            {
                case FileType.PDF:
                    return "application/pdf";
                default:
                    return null;
            }
        }

        public async Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                                  DateTime startDate,
                                                                  DateTime endDate,
                                                                  CancellationToken cancellationToken)
        {
            MessageStatusResponse response = null;

            Smtp2GoEmailSearchRequest apiRequest = new Smtp2GoEmailSearchRequest
                                                   {
                                                       ApiKey = ConfigurationReader.GetValue("SMTP2GoAPIKey"),
                                                       EmailId = new List<String>
                                                                 {
                                                                     providerReference
                                                                 },
                                                       StartDate = startDate.ToString("yyyy-MM-dd"),
                                                       EndDate = endDate.ToString("yyyy-MM-dd"),
                                                   };

            String requestSerialised = JsonConvert.SerializeObject(apiRequest, new JsonSerializerSettings
                                                                               {
                                                                                   TypeNameHandling = TypeNameHandling.None
                                                                               });

            Logger.LogDebug($"Request Message Sent to Email Provider [SMTP2Go] {requestSerialised}");

            StringContent content = new StringContent(requestSerialised, Encoding.UTF8, "application/json");

            String requestUri = $"{ConfigurationReader.GetValue("SMTP2GoBaseAddress")}email/search";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            requestMessage.Content = content;

            HttpResponseMessage httpResponse = await this.HttpClient.SendAsync(requestMessage, cancellationToken);

            Smtp2GoEmailSearchResponse apiResponse = JsonConvert.DeserializeObject<Smtp2GoEmailSearchResponse>(await httpResponse.Content.ReadAsStringAsync());

            Logger.LogDebug($"Response Message Received from Email Provider [SMTP2Go] {JsonConvert.SerializeObject(apiResponse)}");

            // Translate the Response
            response = new MessageStatusResponse
                       {
                           ApiStatusCode = httpResponse.StatusCode,
                           MessageStatus = this.TranslateMessageStatus(apiResponse.Data.EmailDetails.Single().Status),
                           ProviderStatusDescription = apiResponse.Data.EmailDetails.Single().Status,
                           Timestamp = apiResponse.Data.EmailDetails.Single().EmailStatusDate
                       };

            return response;
        }

        private MessageStatus TranslateMessageStatus(String status)
        {
            MessageStatus result;
            switch (status)
            {
                case "failed":
                case "deferred":
                    result = MessageStatus.Failed;
                    break;
                case "hardbounce":
                case "refused":
                case "softbounce":
                case "returned":
                    result = MessageStatus.Bounced;
                    break;
                case "delivered":
                case "ok":
                case "sent":
                    result = MessageStatus.Delivered;
                    break;
                case "rejected":
                    result = MessageStatus.Rejected;
                    break;
                case "complained":
                case "spam":
                    result = MessageStatus.Spam;
                    break;
                default:
                    result = MessageStatus.Unknown;
                    break;
            }

            return result;
        }

        #endregion
    }
}