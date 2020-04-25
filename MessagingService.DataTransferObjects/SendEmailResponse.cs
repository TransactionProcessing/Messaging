namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
        public Guid MessageId { get; set; }

        #endregion
    }
}