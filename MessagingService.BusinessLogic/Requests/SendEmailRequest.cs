namespace MessagingService.BusinessLogic.Requests
{
    using System;
    using System.Collections.Generic;
    using MediatR;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest{System.String}" />
    public class SendEmailRequest : IRequest<String>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailRequest" /> class.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        private SendEmailRequest(Guid connectionIdentifier,
                                 Guid messageId,
                                 String fromAddress,
                                 List<String> toAddresses,
                                 String subject,
                                 String body,
                                 Boolean isHtml)
        {
            this.ConnectionIdentifier = connectionIdentifier;
            this.MessageId = messageId;
            this.FromAddress = fromAddress;
            this.ToAddresses = toAddresses;
            this.Subject = subject;
            this.Body = body;
            this.IsHtml = isHtml;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public String Body { get; }

        /// <summary>
        /// Gets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        public Guid ConnectionIdentifier { get; }

        /// <summary>
        /// Gets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        public String FromAddress { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsHtml { get; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public String Subject { get; }

        /// <summary>
        /// Converts to address.
        /// </summary>
        /// <value>
        /// To address.
        /// </value>
        public List<String> ToAddresses { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified from address.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <returns></returns>
        public static SendEmailRequest Create(Guid connectionIdentifier,
                                              Guid messageId,
                                              String fromAddress,
                                              List<String> toAddresses,
                                              String subject,
                                              String body,
                                              Boolean isHtml)
        {
            return new SendEmailRequest(connectionIdentifier, messageId, fromAddress, toAddresses, subject, body, isHtml);
        }

        #endregion
    }
}