using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    public class Smtp2GoSendEmailResponseData
    {
        /// <summary>
        /// Gets or sets the failed.
        /// </summary>
        /// <value>
        /// The failed.
        /// </value>
        [JsonProperty("failed")]
        public Int32 Failed { get; set; }

        /// <summary>
        /// Gets or sets the failures.
        /// </summary>
        /// <value>
        /// The failures.
        /// </value>
        [JsonProperty("failures")]
        public String[] Failures { get; set; }

        /// <summary>
        /// Gets or sets the email identifier.
        /// </summary>
        /// <value>
        /// The email identifier.
        /// </value>
        [JsonProperty("email_id")]
        public String EmailId { get; set; }

        /// <summary>
        /// Gets or sets the succeesful.
        /// </summary>
        /// <value>
        /// The succeesful.
        /// </value>
        [JsonProperty("succeeded")]
        public Int32 Succeesful { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [JsonProperty("error")]
        public String Error { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("error_code")]
        public String ErrorCode { get; set; }
    }
}