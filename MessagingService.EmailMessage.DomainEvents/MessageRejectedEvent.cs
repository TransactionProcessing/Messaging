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
    public class MessageRejectedEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRejectedEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MessageRejectedEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRejectedEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="rejectedDateTime">The rejected date time.</param>
        private MessageRejectedEvent(Guid aggregateId,
                                     Guid eventId,
                                     String providerStatus,
                                     DateTime rejectedDateTime) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.RejectedDateTime = rejectedDateTime;
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
        /// Gets the rejected date time.
        /// </summary>
        /// <value>
        /// The rejected date time.
        /// </value>
        [JsonProperty]
        public DateTime RejectedDateTime { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="rejectedDateTime">The rejected date time.</param>
        /// <returns></returns>
        public static MessageRejectedEvent Create(Guid aggregateId,
                                                  String providerStatus,
                                                  DateTime rejectedDateTime)
        {
            return new MessageRejectedEvent(aggregateId, Guid.NewGuid(), providerStatus, rejectedDateTime);
        }

        #endregion
    }
}