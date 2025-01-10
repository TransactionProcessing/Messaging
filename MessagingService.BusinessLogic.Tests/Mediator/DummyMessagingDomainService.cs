using MessagingService.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingService.BusinessLogic.Requests;
using SimpleResults;

namespace MessagingService.BusinessLogic.Tests.Mediator
{
    using System.Threading;
    using Models;

    public class DummyMessagingDomainService : IMessagingDomainService
    {
        public async Task<Result<Guid>> SendEmailMessage(Guid connectionIdentifier,
                                                         Guid messageId,
                                                         String fromAddress,
                                                         List<String> toAddresses,
                                                         String subject,
                                                         String body,
                                                         Boolean isHtml,
                                                         List<EmailAttachment> attachments,
                                                         CancellationToken cancellationToken) => Result.Success(Guid.NewGuid());
        
        public async Task<Result<Guid>> SendSMSMessage(Guid connectionIdentifier,
                                                       Guid messageId,
                                                       String sender,
                                                       String destination,
                                                       String message,
                                                       CancellationToken cancellationToken) => Result.Success(Guid.NewGuid());

        public async Task<Result> ResendEmailMessage(Guid connectionIdentifier,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken) => Result.Success();
        public async Task<Result> ResendSMSMessage(Guid connectionIdentifier, Guid messageId, CancellationToken cancellationToken) => Result.Success();

        public async Task<Result> UpdateMessageStatus(EmailCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) => Result.Success();

        public async Task<Result> UpdateMessageStatus(SMSCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) => Result.Success();
    }
}
