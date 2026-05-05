namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksSendSMSRequest
    {
        public String Sender { get; set; }

        public String Destination { get; set; }

        public String Content { get; set; }

        public String Schedule { get; set; }

        public String Tag { get; set; }

        public Int32 Ttl { get; set; }
    }
}