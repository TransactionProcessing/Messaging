namespace MessagingService.EmailMessageAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using EmailMessage.DomainEvents;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.EventStore;
    using Shared.General;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.EventStore.EventStore.Aggregate" />
    /// <seealso cref="Aggregate" />
    public class EmailAggregate : Aggregate
    {
        #region Fields

        /// <summary>
        /// The recipients
        /// </summary>
        private readonly List<MessageRecipient> Recipients;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAggregate" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public EmailAggregate()
        {
            this.Recipients = new List<MessageRecipient>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAggregate" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        private EmailAggregate(Guid aggregateId)
        {
            Guard.ThrowIfInvalidGuid(aggregateId, "Aggregate Id cannot be an Empty Guid");

            this.AggregateId = aggregateId;
            this.MessageId = aggregateId;
            this.Recipients = new List<MessageRecipient>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public String Body { get; private set; }

        /// <summary>
        /// Gets from address.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        public String FromAddress { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsHtml { get; private set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; }

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <value>
        /// The message status.
        /// </value>
        public MessageStatus MessageStatus { get; private set; }

        /// <summary>
        /// Gets the provider email reference.
        /// </summary>
        /// <value>
        /// The provider email reference.
        /// </value>
        public String ProviderEmailReference { get; private set; }

        /// <summary>
        /// Gets the provider request reference.
        /// </summary>
        /// <value>
        /// The provider request reference.
        /// </value>
        public String ProviderRequestReference { get; private set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public String Subject { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <returns></returns>
        public static EmailAggregate Create(Guid aggregateId)
        {
            return new EmailAggregate(aggregateId);
        }

        /// <summary>
        /// Marks the message as bounced.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="bouncedDateTime">The bounced date time.</param>
        public void MarkMessageAsBounced(String providerStatus,
                                         DateTime bouncedDateTime)
        {
            this.CheckMessageCanBeSetToBounced();

            MessageBouncedEvent messageBouncedEvent = MessageBouncedEvent.Create(this.AggregateId, providerStatus, bouncedDateTime);

            this.ApplyAndPend(messageBouncedEvent);
        }

        /// <summary>
        /// Marks the message as delivered.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="deliveredDateTime">The delivered date time.</param>
        public void MarkMessageAsDelivered(String providerStatus,
                                           DateTime deliveredDateTime)
        {
            this.CheckMessageCanBeSetToDelivered();

            MessageDeliveredEvent messageDeliveredEvent = MessageDeliveredEvent.Create(this.AggregateId, providerStatus, deliveredDateTime);

            this.ApplyAndPend(messageDeliveredEvent);
        }

        /// <summary>
        /// Marks the message as failed.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public void MarkMessageAsFailed(String providerStatus,
                                        DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToFailed();

            MessageFailedEvent messageFailedEvent = MessageFailedEvent.Create(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndPend(messageFailedEvent);
        }

        /// <summary>
        /// Marks the message as rejected.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="rejectedDateTime">The rejected date time.</param>
        public void MarkMessageAsRejected(String providerStatus,
                                          DateTime rejectedDateTime)
        {
            this.CheckMessageCanBeSetToRejected();

            MessageRejectedEvent messageRejectedEvent = MessageRejectedEvent.Create(this.AggregateId, providerStatus, rejectedDateTime);

            this.ApplyAndPend(messageRejectedEvent);
        }

        /// <summary>
        /// Marks the message as spam.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="spamDateTime">The spam date time.</param>
        public void MarkMessageAsSpam(String providerStatus,
                                      DateTime spamDateTime)
        {
            this.CheckMessageCanBeSetToSpam();

            MessageMarkedAsSpamEvent messageMarkedAsSpamEvent = MessageMarkedAsSpamEvent.Create(this.AggregateId, providerStatus, spamDateTime);

            this.ApplyAndPend(messageMarkedAsSpamEvent);
        }

        /// <summary>
        /// Messages the send to recipient failure.
        /// </summary>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        public void ReceiveResponseFromProvider(String providerRequestReference,
                                                String providerEmailReference)
        {
            ResponseReceivedFromProviderEvent responseReceivedFromProviderEvent =
                ResponseReceivedFromProviderEvent.Create(this.AggregateId, providerRequestReference, providerEmailReference);

            this.ApplyAndPend(responseReceivedFromProviderEvent);
        }

        /// <summary>
        /// Sends the request to provider.
        /// </summary>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        public void SendRequestToProvider(String fromAddress,
                                          List<String> toAddresses,
                                          String subject,
                                          String body,
                                          Boolean isHtml)
        {
            RequestSentToProviderEvent requestSentToProviderEvent = RequestSentToProviderEvent.Create(this.AggregateId, fromAddress, toAddresses, subject, body, isHtml);

            this.ApplyAndPend(requestSentToProviderEvent);
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        protected override Object GetMetadata()
        {
            return null;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        protected override void PlayEvent(DomainEvent domainEvent)
        {
            this.PlayEvent((dynamic)domainEvent);
        }

        /// <summary>
        /// Checks the message can be set to bounced.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to bounced</exception>
        private void CheckMessageCanBeSetToBounced()
        {
            if (this.MessageStatus != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.MessageStatus} cannot be set to bounced");
            }
        }

        /// <summary>
        /// Checks the message can be set to delivered.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to delivered</exception>
        private void CheckMessageCanBeSetToDelivered()
        {
            if (this.MessageStatus != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.MessageStatus} cannot be set to delivered");
            }
        }

        /// <summary>
        /// Checks the message can be set to failed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to failed</exception>
        private void CheckMessageCanBeSetToFailed()
        {
            if (this.MessageStatus != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.MessageStatus} cannot be set to failed");
            }
        }

        /// <summary>
        /// Checks the message can be set to rejected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to rejected</exception>
        private void CheckMessageCanBeSetToRejected()
        {
            if (this.MessageStatus != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.MessageStatus} cannot be set to rejected");
            }
        }

        /// <summary>
        /// Checks the message can be set to spam.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to spam</exception>
        private void CheckMessageCanBeSetToSpam()
        {
            if (this.MessageStatus != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.MessageStatus} cannot be set to spam");
            }
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(RequestSentToProviderEvent domainEvent)
        {
            this.Body = domainEvent.Body;
            this.Subject = domainEvent.Subject;
            this.IsHtml = domainEvent.IsHtml;
            this.FromAddress = domainEvent.FromAddress;
            this.MessageStatus = MessageStatus.NotSet;

            foreach (String domainEventToAddress in domainEvent.ToAddresses)
            {
                MessageRecipient messageRecipient = new MessageRecipient();
                messageRecipient.Create(domainEventToAddress);
                this.Recipients.Add(messageRecipient);
            }
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(ResponseReceivedFromProviderEvent domainEvent)
        {
            this.ProviderEmailReference = domainEvent.ProviderEmailReference;
            this.ProviderRequestReference = domainEvent.ProviderRequestReference;
            this.MessageStatus = MessageStatus.Sent;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(MessageDeliveredEvent domainEvent)
        {
            this.MessageStatus = MessageStatus.Delivered;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(MessageFailedEvent domainEvent)
        {
            this.MessageStatus = MessageStatus.Failed;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(MessageRejectedEvent domainEvent)
        {
            this.MessageStatus = MessageStatus.Rejected;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(MessageBouncedEvent domainEvent)
        {
            this.MessageStatus = MessageStatus.Bounced;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(MessageMarkedAsSpamEvent domainEvent)
        {
            this.MessageStatus = MessageStatus.Spam;
        }

        #endregion
    }
}