namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record SMSMessageDeliveredEvent : DomainEventRecord.DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSMessageDeliveredEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="deliveredDateTime">The delivered date time.</param>
        public SMSMessageDeliveredEvent(Guid aggregateId,
                                        String providerStatus,
                                        DateTime deliveredDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.DeliveredDateTime = deliveredDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the delivered date time.
        /// </summary>
        /// <value>
        /// The delivered date time.
        /// </value>
        public DateTime DeliveredDateTime { get; init; }

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

        #endregion
    }
}