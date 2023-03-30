namespace MessagingService.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.RequestExamples;
    using DataTransferObjects;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;

    [ExcludeFromCodeCoverage]
    [Route(SMSController.ControllerRoute)]
    [ApiController]
    [Authorize]
    public class SMSController : ControllerBase
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
        public SMSController(IMediator mediator)
        {
            this.Mediator = mediator;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="sendSMSRequest">The send SMS request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Created", typeof(SendSMSResponse))]
        [SwaggerResponseExample(201, typeof(SendSMSResponseExample))]
        public async Task<IActionResult> SendSMS([FromBody] SendSMSRequest sendSMSRequest,
                                                   CancellationToken cancellationToken)
        {
            // Reject password tokens
            if (ClaimsHelper.IsPasswordToken(this.User))
            {
                return this.Forbid();
            }

            Guid messageId = sendSMSRequest.MessageId.HasValue ? sendSMSRequest.MessageId.Value : Guid.NewGuid();

            // Create the command
            BusinessLogic.Requests.SendSMSRequest request = BusinessLogic.Requests.SendSMSRequest.Create(sendSMSRequest.ConnectionIdentifier,
                                                                                                         messageId,
                                                                                                         sendSMSRequest.Sender,
                                                                                                         sendSMSRequest.Destination,
                                                                                                         sendSMSRequest.Message);

            // Route the command
            await this.Mediator.Send(request, cancellationToken);

            // return the result
            return this.Created($"{SMSController.ControllerRoute}/{messageId}",
                                new SendSMSResponse()
                                {
                                    MessageId = messageId
                                });
        }

        [HttpPost]
        [Route("resend")]
        [SwaggerResponse(202, "Accepted", typeof(SendSMSResponse))]
        [SwaggerResponseExample(202, typeof(SendSMSResponseExample))]
        public async Task<IActionResult> ResendSMS([FromBody] ResendSMSRequest resendSMSRequest,
                                                     CancellationToken cancellationToken)
        {
            // Reject password tokens
            if (ClaimsHelper.IsPasswordToken(this.User))
            {
                return this.Forbid();
            }

            // Create the command
            BusinessLogic.Requests.ResendSMSRequest request = BusinessLogic.Requests.ResendSMSRequest.Create(resendSMSRequest.ConnectionIdentifier,
                                                                                                             resendSMSRequest.MessageId);

            // Route the command
            await this.Mediator.Send(request, cancellationToken);

            // return the result
            return this.Accepted($"{SMSController.ControllerRoute}/{resendSMSRequest.MessageId}");
        }

        #endregion

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        public const String ControllerName = "sms";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + SMSController.ControllerName;

        #endregion
    }
}