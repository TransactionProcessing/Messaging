namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEmailServiceProxy
    {
        #region Methods

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
        Task<EmailServiceProxyResponse> SendEmail(Guid messageId,
                                                  String fromAddress,
                                                  List<String> toAddresses,
                                                  String subject,
                                                  String body,
                                                  Boolean isHtml,
                                                  CancellationToken cancellationToken);

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="providerReference">The provider reference.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                     DateTime startDate, 
                                                     DateTime endDate,
                                                     CancellationToken cancellationToken);

        #endregion
    }
}