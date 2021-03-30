namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SendEmailResponse
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