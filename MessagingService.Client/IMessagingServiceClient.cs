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
        Task<Result> SendEmail(String accessToken,
                                          SendEmailRequest request,
                                          CancellationToken cancellationToken);

        Task<Result> ResendEmail(String accessToken,
                                 ResendEmailRequest request,
                                 CancellationToken cancellationToken);

        Task<Result> SendSMS(String accessToken,
                             SendSMSRequest request,
                             CancellationToken cancellationToken);

        #endregion
    }
}