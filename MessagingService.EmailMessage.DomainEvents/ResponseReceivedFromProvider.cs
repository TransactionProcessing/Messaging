namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record ResponseReceivedFromEmailProviderEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseReceivedFromEmailProviderEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        public ResponseReceivedFromEmailProviderEvent(Guid aggregateId,
                                                      String providerRequestReference,
                                                      String providerEmailReference) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.ProviderRequestReference = providerRequestReference;
            this.ProviderEmailReference = providerEmailReference;
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
        /// Gets or sets the provider email reference.
        /// </summary>
        /// <value>
        /// The provider email reference.
        /// </value>
        public String ProviderEmailReference { get; init; }

        /// <summary>
        /// Gets or sets the provider request reference.
        /// </summary>
        /// <value>
        /// The provider request reference.
        /// </value>
        public String ProviderRequestReference { get; init; }

        #endregion
    }
}