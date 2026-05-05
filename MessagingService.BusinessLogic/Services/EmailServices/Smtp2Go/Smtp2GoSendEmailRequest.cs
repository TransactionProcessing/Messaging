using System;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Smtp2GoSendEmailRequest
    {
        public String ApiKey { get; set; }

        public Boolean TestMode { get; set; }

        public String Sender { get; set; }

        public String[] To { get; set; }

        public String[] CC { get; set; }

        public String[] BCC { get; set; }

        public String Subject { get; set; }

        public String HTMLBody { get; set; }

        public String TextBody { get; set; }

        public List<Smtp2GoAttachment> Attachments { get; set; }
    }
}