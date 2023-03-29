namespace MessagingService.EmailMessageAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using EmailMessage.DomainEvents;
    using Models;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.General;
    using Shared.Logger;

    public class EmailAggregate : Aggregate
    {
        #region Fields

        /// <summary>
        /// The recipients
        /// </summary>
        private readonly List<MessageRecipient> Recipients;

        private readonly List<EmailAttachment> Attachments;

        private List<String> ToAddresses;

        public List<String> GetToAddresses() {
            return this.ToAddresses;
        }

        public List<EmailAttachment> GetAttachments()
        {
            return this.Attachments;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAggregate" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public EmailAggregate()
        {
            this.Recipients = new List<MessageRecipient>();
            this.Attachments = new List<EmailAttachment>();
            this.DeliveryStatusList = new List<MessageStatus> {
                                                                  MessageStatus.NotSet
                                                              };
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
            this.Attachments = new List<EmailAttachment>();
            this.DeliveryStatusList = new List<MessageStatus> {
                                                                  MessageStatus.NotSet
                                                              };
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

        public Int32 ResendCount { get; private set; }

        private List<MessageStatus> DeliveryStatusList;

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

            EmailMessageBouncedEvent messageBouncedEvent = new EmailMessageBouncedEvent(this.AggregateId, providerStatus, bouncedDateTime);

            this.ApplyAndAppend(messageBouncedEvent);
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

            EmailMessageDeliveredEvent messageDeliveredEvent = new EmailMessageDeliveredEvent(this.AggregateId, providerStatus, deliveredDateTime);

            this.ApplyAndAppend(messageDeliveredEvent);
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

            EmailMessageFailedEvent messageFailedEvent = new EmailMessageFailedEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageFailedEvent);
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

            EmailMessageRejectedEvent messageRejectedEvent = new EmailMessageRejectedEvent(this.AggregateId, providerStatus, rejectedDateTime);

            this.ApplyAndAppend(messageRejectedEvent);
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

            EmailMessageMarkedAsSpamEvent messageMarkedAsSpamEvent = new EmailMessageMarkedAsSpamEvent(this.AggregateId, providerStatus, spamDateTime);

            this.ApplyAndAppend(messageMarkedAsSpamEvent);
        }

        /// <summary>
        /// Messages the send to recipient failure.
        /// </summary>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        public void ReceiveResponseFromProvider(String providerRequestReference,
                                                String providerEmailReference)
        {
            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent =
                new ResponseReceivedFromEmailProviderEvent(this.AggregateId, providerRequestReference, providerEmailReference);

            this.ApplyAndAppend(responseReceivedFromProviderEvent);
        }
        
        public void SendRequestToProvider(String fromAddress,
                                          List<String> toAddresses,
                                          String subject,
                                          String body,
                                          Boolean isHtml,
                                          List<EmailAttachment> attachments)
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.NotSet)
            {
                throw new InvalidOperationException("Cannot send a message to provider that has already been sent");
            }

            RequestSentToEmailProviderEvent requestSentToProviderEvent = new RequestSentToEmailProviderEvent(this.AggregateId, fromAddress, toAddresses, subject, body, isHtml);

            this.ApplyAndAppend(requestSentToProviderEvent);

            // Record the attachment data
            foreach (EmailAttachment emailAttachment in attachments){
                EmailAttachmentRequestSentToProviderEvent emailAttachmentRequestSentToProviderEvent = new EmailAttachmentRequestSentToProviderEvent(this.AggregateId,
                                                                                                                                                    emailAttachment.Filename,
                                                                                                                                                    emailAttachment.FileData,
                                                                                                                                                    (Int32)emailAttachment.FileType);
                this.ApplyAndAppend(emailAttachmentRequestSentToProviderEvent);
            }
        }

        public void ResendRequestToProvider()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent &&
                this.DeliveryStatusList[this.ResendCount] != MessageStatus.Delivered)
            {
                throw new InvalidOperationException($"Cannot re-send a message to provider that has not already been sent. Current Status [{this.DeliveryStatusList[this.ResendCount]}]");
            }

            RequestResentToEmailProviderEvent requestResentToEmailProviderEvent = new RequestResentToEmailProviderEvent(this.AggregateId);

            this.ApplyAndAppend(requestResentToEmailProviderEvent);
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
        public override void PlayEvent(IDomainEvent domainEvent)
        {
            this.PlayEvent((dynamic)domainEvent);
        }

        private void PlayEvent(Object @event)
        {
            Exception ex = new Exception($"Failed to apply event {@event.GetType()} to Aggregate {this.GetType().Name}");

            Logger.LogCritical(ex);
            throw ex;
        }

        /// <summary>
        /// Checks the message can be set to bounced.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to bounced</exception>
        private void CheckMessageCanBeSetToBounced()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to bounced");
            }
        }

        /// <summary>
        /// Checks the message can be set to delivered.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to delivered</exception>
        private void CheckMessageCanBeSetToDelivered()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to delivered");
            }
        }

        /// <summary>
        /// Checks the message can be set to failed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to failed</exception>
        private void CheckMessageCanBeSetToFailed()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to failed");
            }
        }

        /// <summary>
        /// Checks the message can be set to rejected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to rejected</exception>
        private void CheckMessageCanBeSetToRejected()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to rejected");
            }
        }

        /// <summary>
        /// Checks the message can be set to spam.
        /// </summary>
        /// <exception cref="InvalidOperationException">Message at status {this.MessageStatus} cannot be set to spam</exception>
        private void CheckMessageCanBeSetToSpam()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to spam");
            }
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(RequestSentToEmailProviderEvent domainEvent)
        {
            this.Body = domainEvent.Body;
            this.Subject = domainEvent.Subject;
            this.IsHtml = domainEvent.IsHtml;
            this.FromAddress = domainEvent.FromAddress;
            this.ToAddresses = domainEvent.ToAddresses;
            this.ResendCount = 0;
            this.DeliveryStatusList[this.ResendCount] =MessageStatus.InProgress;

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
        private void PlayEvent(ResponseReceivedFromEmailProviderEvent domainEvent)
        {
            this.ProviderEmailReference = domainEvent.ProviderEmailReference;
            this.ProviderRequestReference = domainEvent.ProviderRequestReference;
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Sent;
        }

        private void PlayEvent(EmailAttachmentRequestSentToProviderEvent domainEvent){
            this.Attachments.Add(new EmailAttachment{
                                                        FileData = domainEvent.FileData,
                                                        FileType = (FileType)domainEvent.FileType,
                                                        Filename = domainEvent.Filename,
                                                    });
        }


        private void PlayEvent(RequestResentToEmailProviderEvent domainEvent) {
            this.ResendCount++;
            this.DeliveryStatusList.Add(MessageStatus.InProgress);
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(EmailMessageDeliveredEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Delivered;
        }

        public MessageStatus GetDeliveryStatus(Int32? resendAttempt = null) {
            if (resendAttempt.HasValue == false) {
                return this.DeliveryStatusList[this.ResendCount];
            }
            return this.DeliveryStatusList[resendAttempt.Value];
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(EmailMessageFailedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Failed;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(EmailMessageRejectedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Rejected;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(EmailMessageBouncedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Bounced;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(EmailMessageMarkedAsSpamEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Spam;
        }

        #endregion
    }
}