namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class SendEmailRequest
    {
        public SendEmailRequest()
        {
            this.EmailAttachments = new List<EmailAttachment>();
        }

        public Guid? MessageId { get; set; }

        public String Body { get; set; }

        public Guid ConnectionIdentifier { get; set; }

        public String FromAddress { get; set; }

        public Boolean IsHtml { get; set; }

        public String Subject { get; set; }

        public List<String> ToAddresses { get; set; }

        public List<EmailAttachment> EmailAttachments { get; set; }
    }
}