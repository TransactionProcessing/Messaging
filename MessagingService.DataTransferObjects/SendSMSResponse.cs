namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    [ExcludeFromCodeCoverage]
    public class SendSMSResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonPropertyName("message_id")]
        public Guid MessageId { get; set; }

        #endregion
    }
}