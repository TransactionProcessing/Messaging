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
    public class MessageExpiredEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExpiredEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MessageExpiredEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExpiredEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="expiredDateTime">The expired date time.</param>
        private MessageExpiredEvent(Guid aggregateId,
                                    Guid eventId,
                                    String providerStatus,
                                    DateTime expiredDateTime) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.ExpiredDateTime = expiredDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounced date time.
        /// </summary>
        /// <value>
        /// The bounced date time.
        /// </value>
        [JsonProperty]
        public DateTime ExpiredDateTime { get; private set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonProperty]
        public Guid MessageId { get; private set; }

        /// <summary>
        /// Gets the provider status.
        /// </summary>
        /// <value>
        /// The provider status.
        /// </value>
        [JsonProperty]
        public String ProviderStatus { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="expiredDateTime">The expired date time.</param>
        /// <returns></returns>
        public static MessageExpiredEvent Create(Guid aggregateId,
                                                 String providerStatus,
                                                 DateTime expiredDateTime)
        {
            return new MessageExpiredEvent(aggregateId, Guid.NewGuid(), providerStatus, expiredDateTime);
        }

        #endregion
    }
}