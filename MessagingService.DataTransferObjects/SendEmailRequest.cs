namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    [ExcludeFromCodeCoverage]
    public class SendEmailRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonPropertyName("message_id")]
        public Guid? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [JsonPropertyName("body")]
        public String Body { get; set; }

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        [JsonPropertyName("connection_identifier")]
        public Guid ConnectionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        [JsonPropertyName("from_address")]
        public String FromAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_html")]
        public Boolean IsHtml { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonPropertyName("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets to addresses.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        [JsonPropertyName("to_addresses")]
        public List<String> ToAddresses { get; set; }

        #endregion
    }
}