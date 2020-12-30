namespace MessagingService.Service.Services.SMSServices.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services.SMSServices;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.EmailServices.IEmailServiceProxy" />
    [ExcludeFromCodeCoverage]
    public class IntegrationTestSMSServiceProxy : ISMSServiceProxy
    {
        #region Methods

        ///// <summary>
        ///// Gets the message status.
        ///// </summary>
        ///// <param name="providerReference">The provider reference.</param>
        ///// <param name="startDate">The start date.</param>
        ///// <param name="endDate">The end date.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <returns></returns>
        //public async Task<MessageStatusResponse> GetMessageStatus(String providerReference,
        //                                                          DateTime startDate,
        //                                                          DateTime endDate,
        //                                                          CancellationToken cancellationToken)
        //{
        //    return new MessageStatusResponse
        //           {
        //               MessageStatus = MessageStatus.Delivered,
        //               ProviderStatusDescription = "delivered"
        //           };
        //}

        #endregion

        public async Task<SMSServiceProxyResponse> SendSMS(Guid messageId,
                                                           String sender,
                                                           String destination,
                                                           String message,
                                                           CancellationToken cancellationToken)
        {
            return new SMSServiceProxyResponse
                   {
                       ApiStatusCode = HttpStatusCode.OK,
                       Error = string.Empty,
                       ErrorCode = string.Empty,
                       SMSIdentifier = "smsid"
                   };
        }
    }
}