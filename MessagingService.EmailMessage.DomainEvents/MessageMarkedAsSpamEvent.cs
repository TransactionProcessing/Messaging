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
    public class MessageMarkedAsSpamEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageMarkedAsSpamEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MessageMarkedAsSpamEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageMarkedAsSpamEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="spamDateTime">The spam date time.</param>
        private MessageMarkedAsSpamEvent(Guid aggregateId,
                                         Guid eventId,
                                         String providerStatus,
                                         DateTime spamDateTime) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.ProviderStatus = providerStatus;
            this.SpamDateTime = spamDateTime;
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
        /// Gets the spam date time.
        /// </summary>
        /// <value>
        /// The spam date time.
        /// </value>
        [JsonProperty]
        public DateTime SpamDateTime { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="spamDateTime">The spam date time.</param>
        /// <returns></returns>
        public static MessageMarkedAsSpamEvent Create(Guid aggregateId,
                                                      String providerStatus,
                                                      DateTime spamDateTime)
        {
            return new MessageMarkedAsSpamEvent(aggregateId, Guid.NewGuid(), providerStatus, spamDateTime);
        }

        #endregion
    }
}