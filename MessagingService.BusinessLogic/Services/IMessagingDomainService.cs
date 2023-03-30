namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    public interface IMessagingDomainService
    {
        #region Methods

        Task SendEmailMessage(Guid connectionIdentifier,
                              Guid messageId,
                              String fromAddress,
                              List<String> toAddresses,
                              String subject,
                              String body,
                              Boolean isHtml,
                              List<EmailAttachment> attachments,
                              CancellationToken cancellationToken);

        Task SendSMSMessage(Guid connectionIdentifier,
                            Guid messageId,
                            String sender,
                            String destination,
                            String message,
                            CancellationToken cancellationToken);

        Task ResendEmailMessage(Guid connectionIdentifier,
                            Guid messageId,
                            CancellationToken cancellationToken);

        Task ResendSMSMessage(Guid connectionIdentifier,
                                Guid messageId,
                                CancellationToken cancellationToken);

        #endregion
    }
}