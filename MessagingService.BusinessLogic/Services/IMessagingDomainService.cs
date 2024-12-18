using SimpleResults;

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

        Task<Result<Guid>> SendEmailMessage(Guid connectionIdentifier,
                                      Guid messageId,
                                      String fromAddress,
                                      List<String> toAddresses,
                                      String subject,
                                      String body,
                                      Boolean isHtml,
                                      List<EmailAttachment> attachments,
                                      CancellationToken cancellationToken);

        Task<Result<Guid>> SendSMSMessage(Guid connectionIdentifier,
                                    Guid messageId,
                                    String sender,
                                    String destination,
                                    String message,
                                    CancellationToken cancellationToken);

        Task<Result> ResendEmailMessage(Guid connectionIdentifier,
                                        Guid messageId,
                                        CancellationToken cancellationToken);

        Task<Result> ResendSMSMessage(Guid connectionIdentifier,
                                Guid messageId,
                                CancellationToken cancellationToken);

        #endregion
    }
}