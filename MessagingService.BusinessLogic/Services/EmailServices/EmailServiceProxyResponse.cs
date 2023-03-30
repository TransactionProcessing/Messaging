namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailServiceProxyResponse
    {
        public Boolean ApiCallSuccessful { get; set; }

        public String RequestIdentifier { get; set; }

        public String EmailIdentifier { get; set; }

        public String ErrorCode { get; set; }

        public String Error { get; set; }
    }
}