namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class ResendEmailRequest
    {
        [JsonProperty("message_id")]
        public Guid MessageId { get; set; }

        [JsonProperty("connection_identifier")]
        public Guid ConnectionIdentifier { get; set; }
    }
}