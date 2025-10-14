namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IEmailServiceProxy
    {
        #region Methods

        Task<EmailServiceProxyResponse> SendEmail(Guid messageId,
                                                  String fromAddress,
                                                  List<String> toAddresses,
                                                  String subject,
                                                  String body,
                                                  Boolean isHtml,
                                                  List<EmailAttachment> attachments,
                                                  CancellationToken cancellationToken);

        Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                     DateTime startDate, 
                                                     DateTime endDate,
                                                     CancellationToken cancellationToken);

        #endregion
    }
}