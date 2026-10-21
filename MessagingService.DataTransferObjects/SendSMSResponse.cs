namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Contains the result identifier for a send-SMS request.</summary>
    [ExcludeFromCodeCoverage]
    public class SendSMSResponse
    {
        /// <summary>Gets or sets the identifier assigned to the message.</summary>
        public Guid MessageId { get; set; }
    }
}
