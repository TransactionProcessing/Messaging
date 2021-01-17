namespace MessagingService.SMSMessage.DomainEvents
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;
    using Shared.DomainDrivenDesign.EventSourcing;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.DomainDrivenDesign.EventSourcing.DomainEvent" />
    public class RequestSentToProviderEvent : DomainEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSentToProviderEvent" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public RequestSentToProviderEvent()
        {
            //We need this for serialisation, so just embrace the DDD crime
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSentToProviderEvent" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        private RequestSentToProviderEvent(Guid aggregateId,
                                           Guid eventId,
                                           String sender,
                                           String destination,
                                           String message) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.Sender = sender;
            this.Destination = destination;
            this.Message = message;
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
        /// Gets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [JsonProperty]
        public String Sender { get; private set; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        [JsonProperty]
        public String Destination { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty]
        public String Message { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static RequestSentToProviderEvent Create(Guid aggregateId,
                                                        String sender,
                                                        String destination,
                                                        String message)
        {
            return new RequestSentToProviderEvent(aggregateId, Guid.NewGuid(), sender,destination,message);
        }

        #endregion
    }
}