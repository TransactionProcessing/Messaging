using MessagingService.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingService.BusinessLogic.Requests;
using TransactionProcessing.UnionResults;

namespace MessagingService.BusinessLogic.Tests.Mediator
{
    using System.Threading;
    using Models;

    public class DummyMessagingDomainService : IMessagingDomainService
    {
        public async Task<Result<Guid>> SendEmailMessage(EmailCommands.SendEmailCommand command,
                                                         List<EmailAttachment> attachments,
                                                         CancellationToken cancellationToken) => Results.Ok(Guid.NewGuid());
        
        public async Task<Result<Guid>> SendSMSMessage(Guid connectionIdentifier,
                                                       Guid messageId,
                                                       String sender,
                                                       String destination,
                                                       String message,
                                                       CancellationToken cancellationToken) => Results.Ok(Guid.NewGuid());

        public async Task<Result> ResendEmailMessage(Guid connectionIdentifier,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken) => Results.Ok();
        public async Task<Result> ResendSMSMessage(Guid connectionIdentifier, Guid messageId, CancellationToken cancellationToken) => Results.Ok();

        public async Task<Result> UpdateMessageStatus(EmailCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) => Results.Ok();

        public async Task<Result> UpdateMessageStatus(SMSCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) => Results.Ok();
    }
}
