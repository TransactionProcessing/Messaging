using MessagingService.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.BusinessLogic.Tests.Mediator
{
    using System.Threading;
    using Models;

    public class DummyMessagingDomainService : IMessagingDomainService
    {
        public async Task SendEmailMessage(Guid connectionIdentifier,
                                           Guid messageId,
                                           String fromAddress,
                                           List<String> toAddresses,
                                           String subject,
                                           String body,
                                           Boolean isHtml,
                                           List<EmailAttachment> attachments,
                                           CancellationToken cancellationToken) {
        }

        public async Task SendSMSMessage(Guid connectionIdentifier,
                                         Guid messageId,
                                         String sender,
                                         String destination,
                                         String message,
                                         CancellationToken cancellationToken) {
        }

        public async Task ResendEmailMessage(Guid connectionIdentifier,
                                             Guid messageId,
                                             CancellationToken cancellationToken) {
        }
    }
}
