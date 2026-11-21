using MediatR;
using Microsoft.AspNetCore.Http;
using MessagingService.BusinessLogic.Requests;
using MessagingService.DataTransferObjects;
using Microsoft.Extensions.Logging;
using TransactionProcessing.UnionResults;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmailAttachment = MessagingService.BusinessLogic.Requests.EmailAttachment;
using FileType = MessagingService.BusinessLogic.Requests.FileType;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MessagingService.Handlers
{
    public static class EmailHandlers
    {
        public static async Task<object> SendEmail(SendEmailRequest sendEmailRequest,
                                                    IMediator mediator,
                                                    CancellationToken cancellationToken) {
            Guid messageId = sendEmailRequest.MessageId.HasValue ? sendEmailRequest.MessageId.Value : Guid.NewGuid();

            List<EmailAttachment> emailAttachments = new();
            foreach (DataTransferObjects.EmailAttachment emailAttachment in sendEmailRequest.EmailAttachments)
            {
                emailAttachments.Add(new EmailAttachment
                {
                    FileData = emailAttachment.FileData,
                    FileType = ConvertFileType(emailAttachment.FileType),
                    Filename = emailAttachment.Filename
                });
            }

            // Create the command
            EmailCommands.SendEmailCommand command = new(sendEmailRequest.ConnectionIdentifier,
                messageId,
                sendEmailRequest.FromAddress,
                sendEmailRequest.ToAddresses,
                sendEmailRequest.Subject,
                sendEmailRequest.Body,
                sendEmailRequest.IsHtml,
                emailAttachments);

            // Route the command
            Result<Guid> result = await mediator.Send(command, cancellationToken);

            return result.Value is Success<Guid> success
                ? HttpResults.Created((string)null, new SendEmailResponse { MessageId = success.Value })
                : result;
        }

        public static async Task<object> ResendEmail(ResendEmailRequest resendEmailRequest,
                                                    IMediator mediator,
                                                    CancellationToken cancellationToken) {
            // Create the command
            EmailCommands.ResendEmailCommand command = new(resendEmailRequest.ConnectionIdentifier,
                resendEmailRequest.MessageId);
            // Route the command
            Result result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess() ? HttpResults.Accepted() : result;
        }

        internal static FileType ConvertFileType(DataTransferObjects.FileType emailAttachmentFileType) {
            switch (emailAttachmentFileType) {
                case DataTransferObjects.FileType.PDF:
                    return FileType.PDF;
                default:
                    return FileType.None;
            }
        }
    }
}
