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

    public static class EmailAggregateExtensions{
        public static void MarkMessageAsBounced(this EmailAggregate aggregate, String providerStatus,
                                         DateTime bouncedDateTime)
        {
            aggregate.CheckMessageCanBeSetToBounced();

            EmailMessageBouncedEvent messageBouncedEvent = new EmailMessageBouncedEvent(aggregate.AggregateId, providerStatus, bouncedDateTime);

            aggregate.ApplyAndAppend(messageBouncedEvent);
        }

        public static void MarkMessageAsDelivered(this EmailAggregate aggregate, String providerStatus,
                                                  DateTime deliveredDateTime)
        {
            aggregate.CheckMessageCanBeSetToDelivered();

            EmailMessageDeliveredEvent messageDeliveredEvent = new EmailMessageDeliveredEvent(aggregate.AggregateId, providerStatus, deliveredDateTime);

            aggregate.ApplyAndAppend(messageDeliveredEvent);
        }

        public static void MarkMessageAsFailed(this EmailAggregate aggregate, String providerStatus,
                                        DateTime failedDateTime)
        {
            aggregate.CheckMessageCanBeSetToFailed();

            EmailMessageFailedEvent messageFailedEvent = new EmailMessageFailedEvent(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageFailedEvent);
        }

        public static void MarkMessageAsRejected(this EmailAggregate aggregate, String providerStatus,
                                          DateTime rejectedDateTime)
        {
            aggregate.CheckMessageCanBeSetToRejected();

            EmailMessageRejectedEvent messageRejectedEvent = new EmailMessageRejectedEvent(aggregate.AggregateId, providerStatus, rejectedDateTime);

            aggregate.ApplyAndAppend(messageRejectedEvent);
        }

        public static void MarkMessageAsSpam(this EmailAggregate aggregate, String providerStatus,
                                      DateTime spamDateTime)
        {
            aggregate.CheckMessageCanBeSetToSpam();

            EmailMessageMarkedAsSpamEvent messageMarkedAsSpamEvent = new EmailMessageMarkedAsSpamEvent(aggregate.AggregateId, providerStatus, spamDateTime);

            aggregate.ApplyAndAppend(messageMarkedAsSpamEvent);
        }

        public static void ReceiveResponseFromProvider(this EmailAggregate aggregate, String providerRequestReference,
                                                String providerEmailReference)
        {
            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent =
                new ResponseReceivedFromEmailProviderEvent(aggregate.AggregateId, providerRequestReference, providerEmailReference);

            aggregate.ApplyAndAppend(responseReceivedFromProviderEvent);
        }

        public static void ReceiveBadResponseFromProvider(this EmailAggregate aggregate, String error, String errorCode)
        {
            BadResponseReceivedFromEmailProviderEvent badResponseReceivedFromProviderEvent =
                new BadResponseReceivedFromEmailProviderEvent(aggregate.AggregateId, errorCode, error);

            aggregate.ApplyAndAppend(badResponseReceivedFromProviderEvent);
        }

        public static MessageStatus GetMessageStatus(this EmailAggregate aggregate)
        {
            return aggregate.DeliveryStatusList[aggregate.ResendCount];
        }

        public static void SendRequestToProvider(this EmailAggregate aggregate, String fromAddress,
                                          List<String> toAddresses,
                                          String subject,
                                          String body,
                                          Boolean isHtml,
                                          List<EmailAttachment> attachments)
        {
            RequestSentToEmailProviderEvent requestSentToProviderEvent = new RequestSentToEmailProviderEvent(aggregate.AggregateId, fromAddress, toAddresses, subject, body, isHtml);

            aggregate.ApplyAndAppend(requestSentToProviderEvent);

            // Record the attachment data
            foreach (EmailAttachment emailAttachment in attachments)
            {
                EmailAttachmentRequestSentToProviderEvent emailAttachmentRequestSentToProviderEvent = new EmailAttachmentRequestSentToProviderEvent(aggregate.AggregateId,
                                                                                                                                                    emailAttachment.Filename,
                                                                                                                                                    emailAttachment.FileData,
                                                                                                                                                    (Int32)emailAttachment.FileType);
                aggregate.ApplyAndAppend(emailAttachmentRequestSentToProviderEvent);
            }
        }

        public static void ResendRequestToProvider(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent &&
                aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Delivered)
            {
                throw new InvalidOperationException($"Cannot re-send a message to provider that has not already been sent. Current Status [{aggregate.DeliveryStatusList[aggregate.ResendCount]}]");
            }

            RequestResentToEmailProviderEvent requestResentToEmailProviderEvent = new RequestResentToEmailProviderEvent(aggregate.AggregateId);

            aggregate.ApplyAndAppend(requestResentToEmailProviderEvent);
        }

        private static void CheckMessageCanBeSetToBounced(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to bounced");
            }
        }

        private static void CheckMessageCanBeSetToDelivered(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to delivered");
            }
        }

        private static void CheckMessageCanBeSetToFailed(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to failed");
            }
        }

        private static void CheckMessageCanBeSetToRejected(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to rejected");
            }
        }

        private static void CheckMessageCanBeSetToSpam(this EmailAggregate aggregate)
        {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to spam");
            }
        }

        public static void PlayEvent(this EmailAggregate aggregate, RequestSentToEmailProviderEvent domainEvent)
        {
            aggregate.Body = domainEvent.Body;
            aggregate.Subject = domainEvent.Subject;
            aggregate.IsHtml = domainEvent.IsHtml;
            aggregate.FromAddress = domainEvent.FromAddress;
            aggregate.ToAddresses = domainEvent.ToAddresses;
            aggregate.ResendCount = 0;
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.InProgress;

            foreach (String domainEventToAddress in domainEvent.ToAddresses)
            {
                MessageRecipient messageRecipient = new MessageRecipient();
                messageRecipient.Create(domainEventToAddress);
                aggregate.Recipients.Add(messageRecipient);
            }
        }

        public static void PlayEvent(this EmailAggregate aggregate, ResponseReceivedFromEmailProviderEvent domainEvent)
        {
            aggregate.ProviderEmailReference = domainEvent.ProviderEmailReference;
            aggregate.ProviderRequestReference = domainEvent.ProviderRequestReference;
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Sent;
        }

        public static void PlayEvent(this EmailAggregate aggregate, BadResponseReceivedFromEmailProviderEvent domainEvent)
        {
            aggregate.Error = domainEvent.Error;
            aggregate.ErrorCode = domainEvent.ErrorCode;
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Failed;
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailAttachmentRequestSentToProviderEvent domainEvent)
        {
            aggregate.Attachments.Add(new EmailAttachment
                                      {
                                          FileData = domainEvent.FileData,
                                          FileType = (FileType)domainEvent.FileType,
                                          Filename = domainEvent.Filename,
                                      });
        }


        public static void PlayEvent(this EmailAggregate aggregate, RequestResentToEmailProviderEvent domainEvent)
        {
            aggregate.ResendCount++;
            aggregate.DeliveryStatusList.Add(MessageStatus.InProgress);
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailMessageDeliveredEvent domainEvent)
        {
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Delivered;
        }

        public static MessageStatus GetDeliveryStatus(this EmailAggregate aggregate, Int32? resendAttempt = null)
        {
            if (resendAttempt.HasValue == false)
            {
                return aggregate.DeliveryStatusList[aggregate.ResendCount];
            }
            return aggregate.DeliveryStatusList[resendAttempt.Value];
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailMessageFailedEvent domainEvent)
        {
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Failed;
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailMessageRejectedEvent domainEvent)
        {
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Rejected;
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailMessageBouncedEvent domainEvent)
        {
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Bounced;
        }

        public static void PlayEvent(this EmailAggregate aggregate, EmailMessageMarkedAsSpamEvent domainEvent)
        {
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Spam;
        }

        public static List<String> GetToAddresses(this EmailAggregate aggregate)
        {
            return aggregate.ToAddresses;
        }

        public static List<EmailAttachment> GetAttachments(this EmailAggregate aggregate)
        {
            return aggregate.Attachments;
        }

        public static List<MessageRecipient> GetRecipients(this EmailAggregate aggregate)
        {
            return aggregate.Recipients;
        }

    }

    public record EmailAggregate : Aggregate
    {
        #region Fields

        internal readonly List<MessageRecipient> Recipients;

        internal readonly List<EmailAttachment> Attachments;

        internal List<String> ToAddresses;
        
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

        public String Body { get; internal set; }

        public String FromAddress { get; internal set; }

        public Boolean IsHtml { get; internal set; }

        public Guid MessageId { get; }

        public String ProviderEmailReference { get; internal set; }

        public String ProviderRequestReference { get; internal set; }

        public String Error { get; internal set; }

        public String ErrorCode { get; internal set; }

        public String Subject { get; internal set; }

        public Int32 ResendCount { get; internal set; }

        internal List<MessageStatus> DeliveryStatusList;

        #endregion

        #region Methods

        public static EmailAggregate Create(Guid aggregateId)
        {
            return new EmailAggregate(aggregateId);
        }
        
        [ExcludeFromCodeCoverage]
        protected override Object GetMetadata()
        {
            return null;
        }

        public override void PlayEvent(IDomainEvent domainEvent) => EmailAggregateExtensions.PlayEvent(this, (dynamic)domainEvent);

        #endregion
    }
}