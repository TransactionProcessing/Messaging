namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TheSMSWorksGetMessageResponse
    {
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        /// <value>
        /// The batch identifier.
        /// </value>
        [JsonProperty("batchid")]
        public String BatchId { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("content")]
        public String Content { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [JsonProperty("created")]
        public String Created { get; set; }

        /// <summary>
        /// Gets or sets the customerid.
        /// </summary>
        /// <value>
        /// The customerid.
        /// </value>
        [JsonProperty("customerid")]
        public String Customerid { get; set; }

        /// <summary>
        /// Gets or sets the deliveryreporturl.
        /// </summary>
        /// <value>
        /// The deliveryreporturl.
        /// </value>
        [JsonProperty("deliveryreporturl")]
        public String Deliveryreporturl { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        [JsonProperty("destination")]
        public String Destination { get; set; }

        /// <summary>
        /// Gets or sets the failurereason.
        /// </summary>
        /// <value>
        /// The failurereason.
        /// </value>
        [JsonProperty("failurereason")]
        public TheSMSWorksFailureReason Failurereason { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("identifier")]
        public String Identifier { get; set; }

        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        /// <value>
        /// The keyword.
        /// </value>
        [JsonProperty("keyword")]
        public String Keyword { get; set; }

        /// <summary>
        /// Gets or sets the messageid.
        /// </summary>
        /// <value>
        /// The messageid.
        /// </value>
        [JsonProperty("messageid")]
        public String Messageid { get; set; }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        [JsonProperty("modified")]
        public String Modified { get; set; }

        /// <summary>
        /// Gets or sets the schedule.
        /// </summary>
        /// <value>
        /// The schedule.
        /// </value>
        [JsonProperty("schedule")]
        public String Schedule { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [JsonProperty("sender")]
        public String Sender { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        [JsonProperty("tag")]
        public String Tag { get; set; }
    }
}