namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record EmailMessageFailedEvent : DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="" /> .
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public EmailMessageFailedEvent(Guid aggregateId,
                                               String providerStatus,
                                               DateTime failedDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.FailedDateTime = failedDateTime;
        }

        /// <summary>
        /// Gets the failed date time.
        /// </summary>
        /// <value>
        /// The failed date time.
        /// </value>
        public DateTime FailedDateTime { get; init; }

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
    }
}