using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MessagingService.BusinessLogic.Requests;
using MessagingService.DataTransferObjects;
using TransactionProcessing.UnionResults;

namespace MessagingService.Handlers;

public static class SMSHandlers {
    public static async Task<Result<SendSMSResponse>> SendSMS(SendSMSRequest sendSMSRequest,
                                              IMediator mediator,
                                              CancellationToken cancellationToken) {
        Guid messageId = sendSMSRequest.MessageId.HasValue ? sendSMSRequest.MessageId.Value : Guid.NewGuid();
        // Create the command
        SMSCommands.SendSMSCommand command = new(sendSMSRequest.ConnectionIdentifier,
            messageId,
            sendSMSRequest.Sender,
            sendSMSRequest.Destination,
            sendSMSRequest.Message);
        // Route the command
        Result<Guid> result = await mediator.Send(command, cancellationToken);
        return result.Match(
            messageId => Result<SendSMSResponse>.Created(new SendSMSResponse { MessageId = messageId }),
            (messageId, location) => Result<SendSMSResponse>.Created(new SendSMSResponse { MessageId = messageId }, location),
            Result<SendSMSResponse>.NoContent,
            Result<SendSMSResponse>.Fail,
            Result<SendSMSResponse>.Fail);
    }

    public static async Task<object> ResendSMS(ResendSMSRequest resendSMSRequest,
                                                IMediator mediator,
                                                CancellationToken cancellationToken) {
        // Create the command
        SMSCommands.ResendSMSCommand command = new(resendSMSRequest.ConnectionIdentifier,
            resendSMSRequest.MessageId);
        // Route the command
        Result result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Results.Accepted() : Result.Fail(new ErrorList(result.Errors));
    }
}
