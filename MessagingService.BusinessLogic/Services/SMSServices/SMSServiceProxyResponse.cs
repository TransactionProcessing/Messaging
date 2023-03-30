namespace MessagingService.BusinessLogic.Services.SMSServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    [ExcludeFromCodeCoverage]
    public class SMSServiceProxyResponse
    {
        public Boolean ApiCallSuccessful { get; set; }

        public String SMSIdentifier { get; set; }

        public String ErrorCode { get; set; }

        public String Error { get; set; }
    }
}