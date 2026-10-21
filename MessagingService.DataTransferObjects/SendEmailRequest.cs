namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>Describes an email message to send.</summary>
    [ExcludeFromCodeCoverage]
    public class SendEmailRequest
    {
        /// <summary>Initializes a new request with an empty attachment collection.</summary>
        public SendEmailRequest()
        {
            this.EmailAttachments = new List<EmailAttachment>();
        }

        /// <summary>Gets or sets an optional caller-supplied message identifier. The service generates an identifier when this value is omitted or null.</summary>
        public Guid? MessageId { get; set; }

        /// <summary>Gets or sets the email body. Its interpretation as plain text or HTML is controlled by <see cref="IsHtml"/>.</summary>
        public String Body { get; set; }

        /// <summary>Gets or sets the identifier of the messaging connection to use.</summary>
        public Guid ConnectionIdentifier { get; set; }

        /// <summary>Gets or sets the sender email address.</summary>
        public String FromAddress { get; set; }

        /// <summary>Gets or sets a value indicating whether <see cref="Body"/> contains HTML.</summary>
        public Boolean IsHtml { get; set; }

        /// <summary>Gets or sets the email subject.</summary>
        public String Subject { get; set; }

        /// <summary>Gets or sets the recipient email addresses.</summary>
        public List<String> ToAddresses { get; set; }

        /// <summary>Gets or sets attachments to include with the email. This collection defaults to empty.</summary>
        public List<EmailAttachment> EmailAttachments { get; set; }
    }
}
