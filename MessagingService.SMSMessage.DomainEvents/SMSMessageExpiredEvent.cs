namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record SMSMessageExpiredEvent : DomainEventRecord.DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSMessageExpiredEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="expiredDateTime">The expired date time.</param>
        public SMSMessageExpiredEvent(Guid aggregateId,
                                      String providerStatus,
                                      DateTime expiredDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.ExpiredDateTime = expiredDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expired date time.
        /// </summary>
        /// <value>
        /// The expired date time.
        /// </value>
        public DateTime ExpiredDateTime { get; init; }

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