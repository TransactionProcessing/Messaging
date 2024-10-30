using System;
using MediatR;
using MessagingService.BusinessLogic.Services.SMSServices;
using SimpleResults;

namespace MessagingService.BusinessLogic.Requests;

public record SMSCommands {
    public record SendSMSCommand(Guid ConnectionIdentifier,
                                 Guid MessageId,
                                 String Sender,
                                 String Destination,
                                 String Message) : IRequest<Result>;

    public record ResendSMSCommand(Guid ConnectionIdentifier,
                                   Guid MessageId) : IRequest<Result>;

    public record UpdateMessageStatusCommand(Guid MessageId,
                                             MessageStatus Status,
                                             String Description,
                                             DateTime Timestamp) : IRequest<Result>;
}