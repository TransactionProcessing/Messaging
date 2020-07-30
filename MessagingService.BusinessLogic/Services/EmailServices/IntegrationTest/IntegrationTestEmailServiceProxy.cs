using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingService.Service.Services.Email.IntegrationTest
{
    using System.Diagnostics.CodeAnalysis;
    using BusinessLogic.Requests;
    using BusinessLogic.Services.EmailServices;

    [ExcludeFromCodeCoverage]
    public class IntegrationTestEmailServiceProxy : IEmailServiceProxy
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<EmailServiceProxyResponse> SendEmail(Guid messageId,
                                                               String fromAddress,
                                                               List<String> toAddresses,
                                                               String subject,
                                                               String body,
                                                               Boolean isHtml, CancellationToken cancellationToken)
        {
            return new EmailServiceProxyResponse
            {
                RequestIdentifier = "requestid",
                EmailIdentifier = "emailid",
                ApiStatusCode = HttpStatusCode.OK,
                Error = String.Empty,
                ErrorCode = String.Empty
            };
        }
    }
}
