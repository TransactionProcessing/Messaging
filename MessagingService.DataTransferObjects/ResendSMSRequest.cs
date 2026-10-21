namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>Identifies an existing SMS message to resend.</summary>
    [ExcludeFromCodeCoverage]
    public class ResendSMSRequest{
        /// <summary>Gets or sets the identifier of the message to resend.</summary>
        public Guid MessageId{ get; set; }

        /// <summary>Gets or sets the identifier of the messaging connection used for the original message.</summary>
        public Guid ConnectionIdentifier{ get; set; }
    }
}
