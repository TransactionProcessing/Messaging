namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TheSMSWorksFailureReason
    {
        public Int32 Code { get; set; }

        public String Details { get; set; }

        public Boolean Permanent { get; set; }
    }
}