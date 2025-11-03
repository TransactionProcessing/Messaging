using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MessagingService.BusinessLogic.Requests;
using MessagingService.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Shared.Results.Web;
using SimpleResults;

namespace MessagingService.Handlers;

public static class SMSHandlers {
    public static async Task<IResult> SendSMS(SendSMSRequest sendSMSRequest,
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
        return ResponseFactory.FromResult<Guid>(result, (r) => new SendSMSResponse
        {
            MessageId = r
        });
    }

    public static async Task<IResult> ResendSMS(ResendSMSRequest resendSMSRequest,
                                                IMediator mediator,
                                                CancellationToken cancellationToken) {
        // Create the command
        SMSCommands.ResendSMSCommand command = new(resendSMSRequest.ConnectionIdentifier,
            resendSMSRequest.MessageId);
        // Route the command
        Result result = await mediator.Send(command, cancellationToken);
        return ResponseFactory.FromResult(result);
    }
}