namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    [ExcludeFromCodeCoverage]
    public class SendSMSRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        [JsonPropertyName("connection_identifier")]
        public Guid ConnectionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        [JsonPropertyName("destination")]
        public String Destination { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonPropertyName("message")]
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonPropertyName("message_id")]
        public Guid? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [JsonPropertyName("sender")]
        public String Sender { get; set; }

        #endregion
    }
}