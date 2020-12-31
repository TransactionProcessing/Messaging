namespace MessagingService.Service.Services.SMSServices.IntegrationTest
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services.SMSServices;
    using MessageStatus = BusinessLogic.Services.SMSServices.MessageStatus;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.SMSServices.ISMSServiceProxy" />
    /// <seealso cref="MessagingService.BusinessLogic.Services.EmailServices.IEmailServiceProxy" />
    [ExcludeFromCodeCoverage]
    public class IntegrationTestSMSServiceProxy : ISMSServiceProxy
    {
        #region Methods

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="providerReference">The provider reference.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                                  CancellationToken cancellationToken)
        {
            return new MessageStatusResponse
                   {
                       MessageStatus = MessageStatus.Delivered,
                       ProviderStatusDescription = "delivered"
                   };
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
                                                           CancellationToken cancellationToken)
        {
            return new SMSServiceProxyResponse
                   {
                       ApiStatusCode = HttpStatusCode.OK,
                       Error = string.Empty,
                       ErrorCode = string.Empty,
                       SMSIdentifier = "smsid"
                   };
        }

        #endregion
    }
}