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
    public class MessageUndeliveredEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageUndeliveredEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MessageUndeliveredEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageUndeliveredEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="undeliveredDateTime">The undelivered date time.</param>
        private MessageUndeliveredEvent(Guid aggregateId,
                                   Guid eventId,
                                   String providerStatus,
                                   DateTime undeliveredDateTime) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.UndeliveredDateTime = undeliveredDateTime;
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
        /// Gets the provider status.
        /// </summary>
        /// <value>
        /// The provider status.
        /// </value>
        [JsonProperty]
        public String ProviderStatus { get; private set; }

        /// <summary>
        /// Gets the undelivered date time.
        /// </summary>
        /// <value>
        /// The undelivered date time.
        /// </value>
        [JsonProperty]
        public DateTime UndeliveredDateTime { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="undeliveredDateTime">The undelivered date time.</param>
        /// <returns></returns>
        public static MessageUndeliveredEvent Create(Guid aggregateId,
                                                String providerStatus,
                                                DateTime undeliveredDateTime)
        {
            return new MessageUndeliveredEvent(aggregateId, Guid.NewGuid(), providerStatus, undeliveredDateTime);
        }

        #endregion
    }
}