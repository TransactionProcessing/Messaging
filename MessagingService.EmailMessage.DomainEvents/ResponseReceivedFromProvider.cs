namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;
    using Shared.DomainDrivenDesign.EventSourcing;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.DomainDrivenDesign.EventSourcing.DomainEvent" />
    public class ResponseReceivedFromProviderEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseReceivedFromProviderEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ResponseReceivedFromProviderEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseReceivedFromProviderEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        private ResponseReceivedFromProviderEvent(Guid aggregateId,
                                                  Guid eventId,
                                                  String providerRequestReference,
                                                  String providerEmailReference) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderRequestReference = providerRequestReference;
            this.ProviderEmailReference = providerEmailReference;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonProperty]
        public Guid MessageId { get; private set; }

        /// <summary>
        /// Gets the provider email reference.
        /// </summary>
        /// <value>
        /// The provider email reference.
        /// </value>
        [JsonProperty]
        public String ProviderEmailReference { get; private set; }

        /// <summary>
        /// Gets the provider request reference.
        /// </summary>
        /// <value>
        /// The provider request reference.
        /// </value>
        [JsonProperty]
        public String ProviderRequestReference { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        /// <returns></returns>
        public static ResponseReceivedFromProviderEvent Create(Guid aggregateId,
                                                               String providerRequestReference,
                                                               String providerEmailReference)
        {
            return new ResponseReceivedFromProviderEvent(aggregateId, Guid.NewGuid(), providerRequestReference, providerEmailReference);
        }

        #endregion
    }
}