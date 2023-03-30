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

        [ExcludeFromCodeCoverage]
        public EmailAggregate()
        {
            this.Recipients = new List<MessageRecipient>();
            this.Attachments = new List<EmailAttachment>();
            this.DeliveryStatusList = new List<MessageStatus> {
                                                                  MessageStatus.NotSet
                                                              };
        }

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

        public String Body { get; private set; }

        public String FromAddress { get; private set; }

        public Boolean IsHtml { get; private set; }

        public Guid MessageId { get; }

        public String ProviderEmailReference { get; private set; }

        public String ProviderRequestReference { get; private set; }

        public String Error { get; private set; }

        public String ErrorCode { get; private set; }

        public String Subject { get; private set; }

        public Int32 ResendCount { get; private set; }

        private List<MessageStatus> DeliveryStatusList;

        #endregion

        #region Methods

        public static EmailAggregate Create(Guid aggregateId)
        {
            return new EmailAggregate(aggregateId);
        }

        public void MarkMessageAsBounced(String providerStatus,
                                         DateTime bouncedDateTime)
        {
            this.CheckMessageCanBeSetToBounced();

            EmailMessageBouncedEvent messageBouncedEvent = new EmailMessageBouncedEvent(this.AggregateId, providerStatus, bouncedDateTime);

            this.ApplyAndAppend(messageBouncedEvent);
        }

        public void MarkMessageAsDelivered(String providerStatus,
                                           DateTime deliveredDateTime)
        {
            this.CheckMessageCanBeSetToDelivered();

            EmailMessageDeliveredEvent messageDeliveredEvent = new EmailMessageDeliveredEvent(this.AggregateId, providerStatus, deliveredDateTime);

            this.ApplyAndAppend(messageDeliveredEvent);
        }

        public void MarkMessageAsFailed(String providerStatus,
                                        DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToFailed();

            EmailMessageFailedEvent messageFailedEvent = new EmailMessageFailedEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageFailedEvent);
        }

        public void MarkMessageAsRejected(String providerStatus,
                                          DateTime rejectedDateTime)
        {
            this.CheckMessageCanBeSetToRejected();

            EmailMessageRejectedEvent messageRejectedEvent = new EmailMessageRejectedEvent(this.AggregateId, providerStatus, rejectedDateTime);

            this.ApplyAndAppend(messageRejectedEvent);
        }

        public void MarkMessageAsSpam(String providerStatus,
                                      DateTime spamDateTime)
        {
            this.CheckMessageCanBeSetToSpam();

            EmailMessageMarkedAsSpamEvent messageMarkedAsSpamEvent = new EmailMessageMarkedAsSpamEvent(this.AggregateId, providerStatus, spamDateTime);

            this.ApplyAndAppend(messageMarkedAsSpamEvent);
        }

        public void ReceiveResponseFromProvider(String providerRequestReference,
                                                String providerEmailReference)
        {
            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent =
                new ResponseReceivedFromEmailProviderEvent(this.AggregateId, providerRequestReference, providerEmailReference);

            this.ApplyAndAppend(responseReceivedFromProviderEvent);
        }

        public void ReceiveBadResponseFromProvider(String error, String errorCode)
        {
            BadResponseReceivedFromEmailProviderEvent badResponseReceivedFromProviderEvent =
                new BadResponseReceivedFromEmailProviderEvent(this.AggregateId, errorCode,error);

            this.ApplyAndAppend(badResponseReceivedFromProviderEvent);
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

        [ExcludeFromCodeCoverage]
        protected override Object GetMetadata()
        {
            return null;
        }

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

        private void CheckMessageCanBeSetToBounced()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to bounced");
            }
        }

        private void CheckMessageCanBeSetToDelivered()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to delivered");
            }
        }

        private void CheckMessageCanBeSetToFailed()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to failed");
            }
        }

        private void CheckMessageCanBeSetToRejected()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to rejected");
            }
        }

        private void CheckMessageCanBeSetToSpam()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to spam");
            }
        }

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

        private void PlayEvent(ResponseReceivedFromEmailProviderEvent domainEvent)
        {
            this.ProviderEmailReference = domainEvent.ProviderEmailReference;
            this.ProviderRequestReference = domainEvent.ProviderRequestReference;
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Sent;
        }

        private void PlayEvent(BadResponseReceivedFromEmailProviderEvent domainEvent)
        {
            this.Error = domainEvent.Error;
            this.ErrorCode = domainEvent.ErrorCode;
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Failed;
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

        private void PlayEvent(EmailMessageFailedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Failed;
        }

        private void PlayEvent(EmailMessageRejectedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Rejected;
        }

        private void PlayEvent(EmailMessageBouncedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Bounced;
        }

        private void PlayEvent(EmailMessageMarkedAsSpamEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Spam;
        }

        #endregion
    }
}