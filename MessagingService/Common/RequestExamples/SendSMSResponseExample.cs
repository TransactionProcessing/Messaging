namespace MessagingService.Common.RequestExamples
{
    using System.Diagnostics.CodeAnalysis;
    using DataTransferObjects;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IExamplesProvider{MessagingService.DataTransferObjects.SendSMSResponse}" />
    [ExcludeFromCodeCoverage]
    public class SendSMSResponseExample : IExamplesProvider<SendSMSResponse>
    {
        #region Methods

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public SendSMSResponse GetExamples()
        {
            return new SendSMSResponse
                   {
                       MessageId = ExampleData.SMSMessageId
                   };
        }

        #endregion
    }
}