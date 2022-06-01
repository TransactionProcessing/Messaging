namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record EmailMessageDeliveredEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="" /> .
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="deliveredDateTime">The delivered date time.</param>
        public EmailMessageDeliveredEvent(Guid aggregateId,
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
        /// Gets the delivered date time.
        /// </summary>
        /// <value>
        /// The delivered date time.
        /// </value>
        public DateTime DeliveredDateTime { get; init; }

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