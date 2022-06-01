namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record SMSMessageUndeliveredEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSMessageUndeliveredEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="undeliveredDateTime">The undelivered date time.</param>
        public SMSMessageUndeliveredEvent(Guid aggregateId,
                                          String providerStatus,
                                          DateTime undeliveredDateTime) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.UndeliveredDateTime = undeliveredDateTime;
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
        /// Gets or sets the undelivered date time.
        /// </summary>
        /// <value>
        /// The undelivered date time.
        /// </value>
        public DateTime UndeliveredDateTime { get; init; }

        #endregion
    }
}