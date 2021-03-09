namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record EmailMessageMarkedAsSpamEvent : DomainEventRecord.DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessageMarkedAsSpamEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="spamDateTime">The spam date time.</param>
        public EmailMessageMarkedAsSpamEvent(Guid aggregateId,
                                             String providerStatus,
                                             DateTime spamDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.SpamDateTime = spamDateTime;
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
        /// Gets or sets the spam date time.
        /// </summary>
        /// <value>
        /// The spam date time.
        /// </value>
        public DateTime SpamDateTime { get; init; }

        #endregion
    }
}