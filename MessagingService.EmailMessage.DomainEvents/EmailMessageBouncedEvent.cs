namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record EmailMessageBouncedEvent : DomainEventRecord.DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="" /> .
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="bouncedDateTime">The bounced date time.</param>
        public EmailMessageBouncedEvent(Guid aggregateId,
                                        String providerStatus,
                                        DateTime bouncedDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.BouncedDateTime = bouncedDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounced date time.
        /// </summary>
        /// <value>
        /// The bounced date time.
        /// </value>
        public DateTime BouncedDateTime { get; init; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; init; }

        /// <summary>
        /// Gets the provider status.
        /// </summary>
        /// <value>
        /// The provider status.
        /// </value>
        public String ProviderStatus { get; init; }

        #endregion
    }
}