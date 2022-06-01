namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record ResponseReceivedFromSMSProviderEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseReceivedFromSMSProviderEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerSMSReference">The provider SMS reference.</param>
        public ResponseReceivedFromSMSProviderEvent(Guid aggregateId,
                                                    String providerSMSReference) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderSMSReference = providerSMSReference;
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
        /// Gets or sets the provider SMS reference.
        /// </summary>
        /// <value>
        /// The provider SMS reference.
        /// </value>
        public String ProviderSMSReference { get; init; }

        #endregion
    }
}