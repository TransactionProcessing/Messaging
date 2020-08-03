namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailDetails
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonProperty("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the delivered at.
        /// </summary>
        /// <value>
        /// The delivered at.
        /// </value>
        [JsonProperty("delivered_at")]
        public DateTime DeliveredAt { get; set; }

        /// <summary>
        /// Gets or sets the email status date.
        /// </summary>
        /// <value>
        /// The email status date.
        /// </value>
        [JsonProperty("email_ts")]
        public DateTime EmailStatusDate { get; set; }

        /// <summary>
        /// Gets or sets the process status.
        /// </summary>
        /// <value>
        /// The process status.
        /// </value>
        [JsonProperty("process_status")]
        public String ProcessStatus { get; set; }

        /// <summary>
        /// Gets or sets the email identifier.
        /// </summary>
        /// <value>
        /// The email identifier.
        /// </value>
        [JsonProperty("email_id")]
        public String EmailId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public String Status { get; set; }
    }
}