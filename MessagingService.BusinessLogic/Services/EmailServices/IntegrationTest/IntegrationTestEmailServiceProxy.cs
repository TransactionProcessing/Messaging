namespace MessagingService.Service.Services.Email.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Requests;
    using BusinessLogic.Services.EmailServices;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.EmailServices.IEmailServiceProxy" />
    [ExcludeFromCodeCoverage]
    public class IntegrationTestEmailServiceProxy : IEmailServiceProxy
    {
        #region Methods

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="providerReference">The provider reference.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                                  DateTime startDate,
                                                                  DateTime endDate,
                                                                  CancellationToken cancellationToken)
        {
            return new MessageStatusResponse
                   {
                       MessageStatus = MessageStatus.Delivered,
                       ProviderStatusDescription = "delivered"
                   };
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EmailServiceProxyResponse> SendEmail(Guid messageId,
                                                               String fromAddress,
                                                               List<String> toAddresses,
                                                               String subject,
                                                               String body,
                                                               Boolean isHtml,
                                                               List<EmailAttachment> attachments,
                                                               CancellationToken cancellationToken)
        {
            return new EmailServiceProxyResponse
                   {
                       RequestIdentifier = "requestid",
                       EmailIdentifier = "emailid",
                       ApiStatusCode = HttpStatusCode.OK,
                       Error = string.Empty,
                       ErrorCode = string.Empty
                   };
        }

        #endregion
    }
}