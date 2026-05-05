namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class TheSmsWorksExtendedErrorModelFieldError
    {
        public String Code { get; set; }

        public String[] Params { get; set; }

        public String Message { get; set; }

        public String[] Path { get; set; }

        public String Description { get; set; }
    }
}