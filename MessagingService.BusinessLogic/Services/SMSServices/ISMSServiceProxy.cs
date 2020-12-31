using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Services.SMSServices
{
    using System.Threading;
    using System.Threading.Tasks;
    using Requests;

    public interface ISMSServiceProxy
    {
        /// <summary>
        /// Sends the SMS.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<SMSServiceProxyResponse> SendSMS(Guid messageId,
                                      String sender,
                                      String destination,
                                      String message,
                                      CancellationToken cancellationToken);

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="providerReference">The provider reference.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<MessageStatusResponse> GetMessageStatus(String providerReference,
                                                     CancellationToken cancellationToken);
    }
}
