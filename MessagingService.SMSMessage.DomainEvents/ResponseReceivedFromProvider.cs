namespace MessagingService.SMSMessage.DomainEvents
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
        /// <param name="providerSMSReference">The provider SMS reference.</param>
        private ResponseReceivedFromProviderEvent(Guid aggregateId,
                                                  Guid eventId,
                                                  String providerSMSReference) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderSMSReference = providerSMSReference;
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
        /// Gets the provider sms reference.
        /// </summary>
        /// <value>
        /// The provider sms reference.
        /// </value>
        [JsonProperty]
        public String ProviderSMSReference { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerSMSReference">The provider SMS reference.</param>
        /// <returns></returns>
        public static ResponseReceivedFromProviderEvent Create(Guid aggregateId,
                                                               String providerSMSReference)
        {
            return new ResponseReceivedFromProviderEvent(aggregateId, Guid.NewGuid(), providerSMSReference);
        }

        #endregion
    }
}