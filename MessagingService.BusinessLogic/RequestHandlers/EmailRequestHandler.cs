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
    public class EmailRequestHandler : IRequestHandler<SendEmailRequest, String>
    {
        #region Fields

        /// <summary>
        /// The email domain service
        /// </summary>
        private readonly IEmailDomainService EmailDomainService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailRequestHandler"/> class.
        /// </summary>
        /// <param name="emailDomainService">The email domain service.</param>
        public EmailRequestHandler(IEmailDomainService emailDomainService)
        {
            this.EmailDomainService = emailDomainService;
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
            await this.EmailDomainService.SendEmailMessage(request.ConnectionIdentifier,
                                                           request.MessageId,
                                                           request.FromAddress,
                                                           request.ToAddresses,
                                                           request.Subject,
                                                           request.Body,
                                                           request.IsHtml,
                                                           cancellationToken);

            return string.Empty;
        }

        #endregion
    }
}