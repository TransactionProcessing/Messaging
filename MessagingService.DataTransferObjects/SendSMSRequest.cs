namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class SendSMSRequest
    {
        public Guid ConnectionIdentifier { get; set; }

        public String Destination { get; set; }

        public String Message { get; set; }

        public Guid? MessageId { get; set; }

        public String Sender { get; set; }
    }
}