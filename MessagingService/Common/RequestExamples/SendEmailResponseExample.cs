namespace MessagingService.Common.RequestExamples
{
    using System.Diagnostics.CodeAnalysis;
    using DataTransferObjects;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IExamplesProvider{MessagingService.DataTransferObjects.SendEmailResponse}" />
    [ExcludeFromCodeCoverage]
    public class SendEmailResponseExample : IExamplesProvider<SendEmailResponse>
    {
        #region Methods

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public SendEmailResponse GetExamples()
        {
            return new SendEmailResponse
                   {
                       MessageId = ExampleData.EmailMessageId
                   };
        }

        #endregion
    }
}