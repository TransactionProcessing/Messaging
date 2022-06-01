namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record SMSMessageRejectedEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSMessageRejectedEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="rejectedDateTime">The rejected date time.</param>
        public SMSMessageRejectedEvent(Guid aggregateId,
                                       String providerStatus,
                                       DateTime rejectedDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.RejectedDateTime = rejectedDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; init; }

        /// <summary>
        /// Gets or sets the provider status.
        /// </summary>
        /// <value>
        /// The provider status.
        /// </value>
        public String ProviderStatus { get; init; }

        /// <summary>
        /// Gets or sets the rejected date time.
        /// </summary>
        /// <value>
        /// The rejected date time.
        /// </value>
        public DateTime RejectedDateTime { get; init; }

        #endregion
    }
}