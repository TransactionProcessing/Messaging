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
    public class Data
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [JsonProperty("count")]
        public Int32 Count { get; set; }

        /// <summary>
        /// Gets or sets the email details.
        /// </summary>
        /// <value>
        /// The email details.
        /// </value>
        [JsonProperty("emails")]
        public List<EmailDetails> EmailDetails { get; set; }
    }
}