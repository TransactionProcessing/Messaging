namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TheSmsWorksTokenRequest
    {
        public String CustomerId { get; set; }

        public String Key { get; set; }

        public String Secret { get; set; }
    }
}