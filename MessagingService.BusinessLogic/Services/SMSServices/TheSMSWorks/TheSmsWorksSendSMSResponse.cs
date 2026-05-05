namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TheSmsWorksSendSMSResponse
    {
        public String MessageId { get; set; }

        public String Status { get; set; }

        public Int32 Credits { get; set; }
    }
}