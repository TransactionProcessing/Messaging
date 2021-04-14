using SendEmailRequestDTO = MessagingService.DataTransferObjects.SendEmailRequest;
using SendEmailResponseDTO = MessagingService.DataTransferObjects.SendEmailResponse;

namespace MessagingService.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.RequestExamples;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using SendEmailRequest = BusinessLogic.Requests.SendEmailRequest;

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
        [ProducesResponseType(typeof(SendEmailResponseDTO), 201)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDTO sendEmailRequest,
                                                   CancellationToken cancellationToken)
        {
            // Reject password tokens
            if (ClaimsHelper.IsPasswordToken(this.User))
            {
                return this.Forbid();
            }

            Guid messageId =  sendEmailRequest.MessageId.HasValue ? sendEmailRequest.MessageId.Value : Guid.NewGuid();

            // Create the command
            SendEmailRequest request = SendEmailRequest.Create(sendEmailRequest.ConnectionIdentifier,
                                                               messageId,
                                                               sendEmailRequest.FromAddress,
                                                               sendEmailRequest.ToAddresses,
                                                               sendEmailRequest.Subject,
                                                               sendEmailRequest.Body,
                                                               sendEmailRequest.IsHtml);

            // Route the command
            await this.Mediator.Send(request, cancellationToken);

            // return the result
            return this.Created($"{EmailController.ControllerRoute}/{messageId}",
                                new SendEmailResponseDTO
                                {
                                    MessageId = messageId
                                });
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