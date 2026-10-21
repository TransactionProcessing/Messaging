namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>Contains the result identifier for a send-email request.</summary>
    [ExcludeFromCodeCoverage]
    public class SendEmailResponse
    {
        /// <summary>Gets or sets the identifier assigned to the message.</summary>
        public Guid MessageId { get; set; }
    }
}
