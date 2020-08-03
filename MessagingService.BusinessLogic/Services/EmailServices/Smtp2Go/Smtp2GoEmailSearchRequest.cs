namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Smtp2GoEmailSearchRequest
    {
        /// <summary>
        /// Gets or sets the email identifier.
        /// </summary>
        /// <value>
        /// The email identifier.
        /// </value>
        [JsonProperty("email_id")]
        public List<String> EmailId { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        [JsonProperty("api_key")]
        public String ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        [JsonProperty("start_date")]
        public String StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [JsonProperty("end_date")]
        public String EndDate { get; set; }
    }
}