namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class ResendSMSRequest{
        public Guid MessageId{ get; set; }

        public Guid ConnectionIdentifier{ get; set; }
    }
}