using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MessagingService.BusinessLogic.Requests;
using MessagingService.DataTransferObjects;
using TransactionProcessing.UnionResults;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace MessagingService.Handlers;

public static class SMSHandlers {
    public static async Task<object> SendSMS(SendSMSRequest sendSMSRequest,
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
        return result.Value is Success<Guid> success
            ? HttpResults.Created((string)null, new SendSMSResponse { MessageId = success.Value })
            : result;
    }

    public static async Task<object> ResendSMS(ResendSMSRequest resendSMSRequest,
                                                IMediator mediator,
                                                CancellationToken cancellationToken) {
        // Create the command
        SMSCommands.ResendSMSCommand command = new(resendSMSRequest.ConnectionIdentifier,
            resendSMSRequest.MessageId);
        // Route the command
        Result result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess() ? HttpResults.Accepted() : result;
    }
}
