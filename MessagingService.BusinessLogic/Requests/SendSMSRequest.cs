namespace MessagingService.BusinessLogic.Requests
{
    using System;
    using MediatR;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest{System.String}" />
    public class SendSMSRequest : IRequest
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailRequest" /> class.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        private SendSMSRequest(Guid connectionIdentifier,
                               Guid messageId,
                               String sender,
                               String destination,
                               String message)
        {
            this.ConnectionIdentifier = connectionIdentifier;
            this.MessageId = messageId;
            this.Sender = sender;
            this.Destination = destination;
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        public Guid ConnectionIdentifier { get; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public String Sender { get; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        public String Destination { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public String Message { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified from address.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static SendSMSRequest Create(Guid connectionIdentifier,
                                            Guid messageId,
                                            String sender,
                                            String destination,
                                            String message)
        {
            return new SendSMSRequest(connectionIdentifier, messageId, sender, destination,message);
        }

        #endregion
    }
}