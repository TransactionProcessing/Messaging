namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksTokenRequest
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        [JsonProperty("customerid")]
        public String CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [JsonProperty("key")]
        public String Key { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>
        /// The secret.
        /// </value>
        [JsonProperty("secret")]
        public String Secret { get; set; }
    }
}