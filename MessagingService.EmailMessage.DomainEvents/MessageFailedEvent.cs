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
    public class MessageFailedEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFailedEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MessageFailedEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFailedEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        private MessageFailedEvent(Guid aggregateId,
                                   Guid eventId,
                                   String providerStatus,
                                   DateTime failedDateTime) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.FailedDateTime = failedDateTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the failed date time.
        /// </summary>
        /// <value>
        /// The failed date time.
        /// </value>
        [JsonProperty]
        public DateTime FailedDateTime { get; private set; }

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
        /// <param name="failedDateTime">The failed date time.</param>
        /// <returns></returns>
        public static MessageFailedEvent Create(Guid aggregateId,
                                                String providerStatus,
                                                DateTime failedDateTime)
        {
            return new MessageFailedEvent(aggregateId, Guid.NewGuid(), providerStatus, failedDateTime);
        }

        #endregion
    }
}