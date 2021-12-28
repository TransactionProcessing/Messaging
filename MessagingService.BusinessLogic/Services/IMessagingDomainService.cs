﻿namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Requests;

    /// <summary>
    /// 
    /// </summary>
    public interface IMessagingDomainService
    {
        #region Methods

        /// <summary>
        /// Sends the email message.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SendEmailMessage(Guid connectionIdentifier,
                              Guid messageId,
                              String fromAddress,
                              List<String> toAddresses,
                              String subject,
                              String body,
                              Boolean isHtml,
                              List<EmailAttachment> attachments,
                              CancellationToken cancellationToken);

        /// <summary>
        /// Sends the SMS message.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SendSMSMessage(Guid connectionIdentifier,
                            Guid messageId,
                            String sender,
                            String destination,
                            String message,
                            CancellationToken cancellationToken);

        #endregion
    }
}