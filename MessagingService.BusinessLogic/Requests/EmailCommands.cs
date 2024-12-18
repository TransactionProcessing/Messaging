using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using MessagingService.BusinessLogic.Services.EmailServices;
using SimpleResults;

namespace MessagingService.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record EmailCommands {
    public record SendEmailCommand(Guid ConnectionIdentifier,
                                   Guid MessageId,
                                   String FromAddress,
                                   List<String> ToAddresses,
                                   String Subject,
                                   String Body,
                                   Boolean IsHtml,
                                   List<EmailAttachment> EmailAttachments) : IRequest<Result<Guid>>;

    public record ResendEmailCommand(Guid ConnectionIdentifier,
                                     Guid MessageId) : IRequest<Result>;

    public record UpdateMessageStatusCommand(Guid MessageId,
                                      MessageStatus Status,
                                      String Description,
                                      DateTime Timestamp) : IRequest<Result>;
}