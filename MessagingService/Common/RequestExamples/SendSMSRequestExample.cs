namespace MessagingService.Common.RequestExamples
{
    using System.Diagnostics.CodeAnalysis;
    using DataTransferObjects;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IExamplesProvider{MessagingService.DataTransferObjects.SendSMSRequest}" />
    [ExcludeFromCodeCoverage]
    public class SendSMSRequestExample : IExamplesProvider<SendSMSRequest>
    {
        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public SendSMSRequest GetExamples()
        {
            return new SendSMSRequest
                   {
                       ConnectionIdentifier = ExampleData.ConnectionIdentifier,
                       MessageId = ExampleData.SMSMessageId,
                       Destination = ExampleData.SMSMessageDestination,
                       Message = ExampleData.SMSMessageMessage,
                       Sender = ExampleData.SMSMessageSender
                   };
        }
    }
}