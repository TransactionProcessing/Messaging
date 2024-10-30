using SimpleResults;
using SendEmailRequestDTO = MessagingService.DataTransferObjects.SendEmailRequest;
using SendEmailResponseDTO = MessagingService.DataTransferObjects.SendEmailResponse;
using ResendEmailRequestDTO = MessagingService.DataTransferObjects.ResendEmailRequest;

namespace MessagingService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Requests;
    using Common;
    using Common.RequestExamples;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ExcludeFromCodeCoverage]
    [Route(EmailController.ControllerRoute)]
    [ApiController]
    [Authorize]
    public class EmailController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator Mediator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public EmailController(IMediator mediator)
        {
            this.Mediator = mediator;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="sendEmailRequest">The send email request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Created", typeof(SendEmailResponseDTO))]
        [SwaggerResponseExample(201, typeof(SendEmailResponseExample))]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDTO sendEmailRequest,
                                                   CancellationToken cancellationToken)
        {
            // Reject password tokens
            if (ClaimsHelper.IsPasswordToken(this.User))
            {
                return this.Forbid();
            }

            Guid messageId =  sendEmailRequest.MessageId.HasValue ? sendEmailRequest.MessageId.Value : Guid.NewGuid();

            List<EmailAttachment> emailAttachments = new List<EmailAttachment>();
            foreach (DataTransferObjects.EmailAttachment emailAttachment in sendEmailRequest.EmailAttachments)
            {
                emailAttachments.Add(new EmailAttachment
                                     {
                                         FileData = emailAttachment.FileData,
                                         FileType = this.ConvertFileType(emailAttachment.FileType),
                                         Filename = emailAttachment.Filename
                                     });
            }

            // Create the command
            EmailCommands.SendEmailCommand command= new(sendEmailRequest.ConnectionIdentifier,
                                                               messageId,
                                                               sendEmailRequest.FromAddress,
                                                               sendEmailRequest.ToAddresses,
                                                               sendEmailRequest.Subject,
                                                               sendEmailRequest.Body,
                                                               sendEmailRequest.IsHtml,
                                                               emailAttachments);

            // Route the command
            var result = await this.Mediator.Send(command, cancellationToken);

            // return the result
            return this.Created($"{EmailController.ControllerRoute}/{messageId}",
                                new SendEmailResponseDTO
                                {
                                    MessageId = messageId
                                });
        }

        [HttpPost]
        [Route("resend")]
        [SwaggerResponse(202, "Accepted", typeof(SendEmailResponseDTO))]
        [SwaggerResponseExample(202, typeof(SendEmailResponseExample))]
        public async Task<IActionResult> ResendEmail([FromBody] ResendEmailRequestDTO resendEmailRequest,
                                                   CancellationToken cancellationToken)
        {
            // Reject password tokens
            if (ClaimsHelper.IsPasswordToken(this.User))
            {
                return this.Forbid();
            }

            // Create the command
            EmailCommands.ResendEmailCommand command = new(resendEmailRequest.ConnectionIdentifier, resendEmailRequest.MessageId);
            
            // Route the command
            Result result = await this.Mediator.Send(command, cancellationToken);

            // return the result
            return this.Accepted($"{EmailController.ControllerRoute}/{resendEmailRequest.MessageId}");
        }

        private FileType ConvertFileType(DataTransferObjects.FileType emailAttachmentFileType)
        {
            switch(emailAttachmentFileType)
            {
                case DataTransferObjects.FileType.PDF:
                    return MessagingService.BusinessLogic.Requests.FileType.PDF;
                default:
                    return MessagingService.BusinessLogic.Requests.FileType.None;
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        public const String ControllerName = "email";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + EmailController.ControllerName;

        #endregion
    }
}