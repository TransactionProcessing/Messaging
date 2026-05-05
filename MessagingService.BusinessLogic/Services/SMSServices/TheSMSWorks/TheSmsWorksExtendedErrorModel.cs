namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksExtendedErrorModel
    {
        public String Message { get; set; }

        public TheSmsWorksExtendedErrorModelError[] Errors { get; set; }
    }
}