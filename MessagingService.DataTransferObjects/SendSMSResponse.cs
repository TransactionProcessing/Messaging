namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class SendSMSResponse
    {
        public Guid MessageId { get; set; }
    }
}