namespace MessagingService.EmailMessage.DomainEvents
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
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        private RequestSentToProviderEvent(Guid aggregateId,
                                           Guid eventId,
                                           String fromAddress,
                                           List<String> toAddresses,
                                           String subject,
                                           String body,
                                           Boolean isHtml) : base(aggregateId, eventId)
        {
            this.MessageId = aggregateId;
            this.FromAddress = fromAddress;
            this.ToAddresses = toAddresses;
            this.Subject = subject;
            this.Body = body;
            this.IsHtml = isHtml;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [JsonProperty]
        public String Body { get; private set; }

        /// <summary>
        /// Gets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        [JsonProperty]
        public String FromAddress { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty]
        public Boolean IsHtml { get; private set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        [JsonProperty]
        public Guid MessageId { get; private set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonProperty]
        public String Subject { get; private set; }

        /// <summary>
        /// Converts to addresses.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        [JsonProperty]
        public List<String> ToAddresses { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <returns></returns>
        public static RequestSentToProviderEvent Create(Guid aggregateId,
                                                        String fromAddress,
                                                        List<String> toAddresses,
                                                        String subject,
                                                        String body,
                                                        Boolean isHtml)
        {
            return new RequestSentToProviderEvent(aggregateId, Guid.NewGuid(), fromAddress, toAddresses, subject, body, isHtml);
        }

        #endregion
    }
}