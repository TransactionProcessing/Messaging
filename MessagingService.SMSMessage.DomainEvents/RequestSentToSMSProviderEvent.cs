namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record RequestSentToSMSProviderEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSentToSMSProviderEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        public RequestSentToSMSProviderEvent(Guid aggregateId,
                                             String sender,
                                             String destination,
                                             String message) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.Sender = sender;
            this.Destination = destination;
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        public String Destination { get; init; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public String Message { get; init; }

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; init; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public String Sender { get; init; }

        #endregion
    }
}