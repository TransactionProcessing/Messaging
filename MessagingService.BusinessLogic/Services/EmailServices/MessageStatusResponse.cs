namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageStatusResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the API status code.
        /// </summary>
        /// <value>
        /// The API status code.
        /// </value>
        public HttpStatusCode ApiStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the message status.
        /// </summary>
        /// <value>
        /// The message status.
        /// </value>
        public MessageStatus MessageStatus { get; set; }

        /// <summary>
        /// Gets or sets the provider status description.
        /// </summary>
        /// <value>
        /// The provider status description.
        /// </value>
        public String ProviderStatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }

        #endregion
    }
}