namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksExtendedErrorModelError
    {
        public String Code { get; set; }

        public TheSmsWorksExtendedErrorModelFieldError[] FieldErrors { get; set; }

        public String In { get; set; }

        public String Message { get; set; }

        public String Name { get; set; }

        public String[] Path { get; set; }
    }
}