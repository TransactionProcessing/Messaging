using SimpleResults;

namespace MessagingService.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;

    /// <summary>
    /// 
    /// </summary>
    public interface IMessagingServiceClient
    {
        #region Methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Result> SendEmail(String accessToken,
                                          SendEmailRequest request,
                                          CancellationToken cancellationToken);

        Task<Result> ResendEmail(String accessToken,
                                 ResendEmailRequest request,
                                 CancellationToken cancellationToken);

        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Result> SendSMS(String accessToken,
                             SendSMSRequest request,
                             CancellationToken cancellationToken);

        #endregion
    }
}