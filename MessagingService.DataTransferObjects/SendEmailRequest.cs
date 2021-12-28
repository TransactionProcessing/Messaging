namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class SendEmailRequest
    {
        public SendEmailRequest()
        {
            this.EmailAttachments = new List<EmailAttachment>();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonProperty("message_id")]
        public Guid? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [JsonProperty("body")]
        public String Body { get; set; }

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        [JsonProperty("connection_identifier")]
        public Guid ConnectionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        [JsonProperty("from_address")]
        public String FromAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("is_html")]
        public Boolean IsHtml { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonProperty("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets to addresses.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        [JsonProperty("to_addresses")]
        public List<String> ToAddresses { get; set; }

        /// <summary>
        /// Gets or sets the email attachments.
        /// </summary>
        /// <value>
        /// The email attachments.
        /// </value>
        [JsonProperty("email_attachements")]
        public List<EmailAttachment> EmailAttachments { get; set; }

        #endregion
    }
}