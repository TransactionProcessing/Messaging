using System;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Smtp2GoSendEmailResponse
    {
        public String RequestId { get; set; }

        public Smtp2GoSendEmailResponseData Data { get; set; }
    }
}