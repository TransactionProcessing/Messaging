namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class SendEmailResponse
    {
        public Guid MessageId { get; set; }
    }
}