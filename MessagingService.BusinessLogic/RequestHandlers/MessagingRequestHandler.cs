namespace MessagingService.BusinessLogic.RequestHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Requests;
    using Services;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler{MessagingService.BusinessLogic.Requests.SendEmailRequest, System.String}" />
    public class MessagingRequestHandler : IRequestHandler<SendEmailRequest, String>, 
                                           IRequestHandler<SendSMSRequest, String>,
                                           IRequestHandler<ResendEmailRequest,String>
    {
        #region Fields

        /// <summary>
        /// The messaging domain service
        /// </summary>
        private readonly IMessagingDomainService MessagingDomainService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingRequestHandler" /> class.
        /// </summary>
        /// <param name="messagingDomainService">The messaging domain service.</param>
        public MessagingRequestHandler(IMessagingDomainService messagingDomainService)
        {
            this.MessagingDomainService = messagingDomainService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<String> Handle(SendEmailRequest request,
                                         CancellationToken cancellationToken)
        {
            await this.MessagingDomainService.SendEmailMessage(request.ConnectionIdentifier,
                                                               request.MessageId,
                                                               request.FromAddress,
                                                               request.ToAddresses,
                                                               request.Subject,
                                                               request.Body,
                                                               request.IsHtml,
                                                               request.EmailAttachments,
                                                               cancellationToken);

            return string.Empty;
        }

        public async Task<String> Handle(SendSMSRequest request,
                                         CancellationToken cancellationToken)
        {
            await this.MessagingDomainService.SendSMSMessage(request.ConnectionIdentifier,
                                                             request.MessageId,
                                                             request.Sender,
                                                             request.Destination,
                                                             request.Message,
                                                             cancellationToken);
            return string.Empty;
        }

        #endregion

        public async Task<String> Handle(ResendEmailRequest request,
                                   CancellationToken cancellationToken) {
            await this.MessagingDomainService.ResendEmailMessage(request.ConnectionIdentifier, request.MessageId, cancellationToken);

            return String.Empty;
        }
    }
}