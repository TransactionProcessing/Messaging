using System;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Smtp2GoSendEmailResponseData
    {
        public Int32 Failed { get; set; }

        public String[] Failures { get; set; }

        public String EmailId { get; set; }

        public Int32 Succeesful { get; set; }

        public String Error { get; set; }

        public String ErrorCode { get; set; }
    }
}