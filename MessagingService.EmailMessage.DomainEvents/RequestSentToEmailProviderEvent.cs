namespace MessagingService.EmailMessage.DomainEvents
{
    using System;
    using System.Collections.Generic;
    using Shared.DomainDrivenDesign.EventSourcing;

    public record RequestSentToEmailProviderEvent : DomainEventRecord.DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSentToEmailProviderEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        public RequestSentToEmailProviderEvent(Guid aggregateId,
                                           String fromAddress,
                                           List<String> toAddresses,
                                           String subject,
                                           String body,
                                           Boolean isHtml) : base(aggregateId, Guid.NewGuid())
        {
            this.MessageId = aggregateId;
            this.FromAddress = fromAddress;
            this.ToAddresses = toAddresses;
            this.Subject = subject;
            this.Body = body;
            this.IsHtml = isHtml;
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public String Body { get; init; }
        /// <summary>
        /// Gets or sets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        public String FromAddress { get; init; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsHtml { get; init; }
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; init; }


        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public String Subject { get; init; }


        /// <summary>
        /// Converts to addresses.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        public List<String> ToAddresses { get; init; }
    }
}