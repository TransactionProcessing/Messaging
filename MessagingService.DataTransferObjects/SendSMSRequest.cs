namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>Describes an SMS message to send.</summary>
    [ExcludeFromCodeCoverage]
    public class SendSMSRequest
    {
        /// <summary>Gets or sets the identifier of the messaging connection to use.</summary>
        public Guid ConnectionIdentifier { get; set; }

        /// <summary>Gets or sets the recipient phone number.</summary>
        public String Destination { get; set; }

        /// <summary>Gets or sets the text content of the SMS message.</summary>
        public String Message { get; set; }

        /// <summary>Gets or sets an optional caller-supplied message identifier. The service generates an identifier when this value is omitted or null.</summary>
        public Guid? MessageId { get; set; }

        /// <summary>Gets or sets the sender name or number shown to the recipient.</summary>
        public String Sender { get; set; }
    }
}
