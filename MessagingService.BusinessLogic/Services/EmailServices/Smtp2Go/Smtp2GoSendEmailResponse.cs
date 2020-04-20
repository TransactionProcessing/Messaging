using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    public class Smtp2GoSendEmailResponse
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("request_id")]
        public String RequestId { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonProperty("data")]
        public Smtp2GoSendEmailResponseData Data { get; set; }
    }
}