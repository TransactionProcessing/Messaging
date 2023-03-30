using System;

namespace MessagingService.SMSMessageAggregate
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.General;
    using Shared.Logger;
    using SMSMessage.DomainEvents;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Aggregate" />
    public class SMSAggregate : Aggregate
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSAggregate" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public SMSAggregate()
        {
            this.DeliveryStatusList = new List<MessageStatus> {
                                                                  MessageStatus.NotSet
                                                              };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SMSAggregate" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        private SMSAggregate(Guid aggregateId)
        {
            Guard.ThrowIfInvalidGuid(aggregateId, "Aggregate Id cannot be an Empty Guid");

            this.AggregateId = aggregateId;
            this.MessageId = aggregateId;
            this.DeliveryStatusList = new List<MessageStatus> {
                                                                  MessageStatus.NotSet
                                                              };
        }

        #endregion

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; private set; }
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public String Sender { get; private set; }
        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        public String Destination { get; private set; }
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public String Message { get; private set; }
        /// <summary>
        /// Gets the provider reference.
        /// </summary>
        /// <value>
        /// The provider reference.
        /// </value>
        public String ProviderReference { get; private set; }

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <value>
        /// The message status.
        /// </value>
        //public MessageStatus MessageStatus { get; private set; }

        /// <summary>
        /// Creates the specified aggregate identifier.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <returns></returns>
        public static SMSAggregate Create(Guid aggregateId)
        {
            return new SMSAggregate(aggregateId);
        }

        /// <summary>
        /// Messages the send to recipient failure.
        /// </summary>
        /// <param name="providerSMSReference">The provider SMS reference.</param>
        public void ReceiveResponseFromProvider(String providerSMSReference)
        {
            ResponseReceivedFromSMSProviderEvent responseReceivedFromProviderEvent =
                new ResponseReceivedFromSMSProviderEvent(this.AggregateId, providerSMSReference);

            this.ApplyAndAppend(responseReceivedFromProviderEvent);
        }

        /// <summary>
        /// Sends the request to provider.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.InvalidOperationException">Cannot send a message to provider that has already been sent</exception>
        public void SendRequestToProvider(String sender,
                                          String destination,
                                          String message)
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.NotSet)
            {
                throw new InvalidOperationException("Cannot send a message to provider that has already been sent");
            }

            RequestSentToSMSProviderEvent requestSentToProviderEvent = new RequestSentToSMSProviderEvent(this.AggregateId, sender,destination,message);

            this.ApplyAndAppend(requestSentToProviderEvent);
        }

        /// <summary>
        /// Marks the message as failed.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public void MarkMessageAsExpired(String providerStatus,
                                        DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToExpired();

            SMSMessageExpiredEvent messageExpiredEvent = new SMSMessageExpiredEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageExpiredEvent);
        }

        /// <summary>
        /// Marks the message as failed.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public void MarkMessageAsRejected(String providerStatus,
                                          DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToRejected();

            SMSMessageRejectedEvent messageRejectedEvent = new SMSMessageRejectedEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageRejectedEvent);
        }

        /// <summary>
        /// Marks the message as delivered.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public void MarkMessageAsDelivered(String providerStatus,
                                           DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToDelivered();

            SMSMessageDeliveredEvent messageDeliveredEvent = new SMSMessageDeliveredEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageDeliveredEvent);
        }


        /// <summary>
        /// Marks the message as undeliverable.
        /// </summary>
        /// <param name="providerStatus">The provider status.</param>
        /// <param name="failedDateTime">The failed date time.</param>
        public void MarkMessageAsUndeliverable(String providerStatus,
                                               DateTime failedDateTime)
        {
            this.CheckMessageCanBeSetToUndeliverable();

            SMSMessageUndeliveredEvent messageUndeliveredEvent = new SMSMessageUndeliveredEvent(this.AggregateId, providerStatus, failedDateTime);

            this.ApplyAndAppend(messageUndeliveredEvent);
        }

        /// <summary>
        /// Checks the message can be set to expired.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Message at status {this.MessageStatus} cannot be set to expired</exception>
        private void CheckMessageCanBeSetToExpired()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to expired");
            }
        }

        /// <summary>
        /// Checks the message can be set to rejected.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Message at status {this.MessageStatus} cannot be set to rejected</exception>
        private void CheckMessageCanBeSetToRejected()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to rejected");
            }
        }
        /// <summary>
        /// Checks the message can be set to delivered.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Message at status {this.MessageStatus} cannot be set to delivered</exception>
        private void CheckMessageCanBeSetToDelivered()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to delivered");
            }
        }

        /// <summary>
        /// Checks the message can be set to undeliverable.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Message at status {this.MessageStatus} cannot be set to undeliverable</exception>
        private void CheckMessageCanBeSetToUndeliverable()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent)
            {
                throw new InvalidOperationException($"Message at status {this.DeliveryStatusList[this.ResendCount]} cannot be set to undeliverable");
            }
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
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(RequestSentToSMSProviderEvent domainEvent)
        {
            this.Sender = domainEvent.Sender;
            this.Destination = domainEvent.Destination;
            this.Message = domainEvent.Message;
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.InProgress;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(ResponseReceivedFromSMSProviderEvent domainEvent)
        {
            this.ProviderReference = domainEvent.ProviderSMSReference;
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Sent;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(SMSMessageExpiredEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Expired;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(SMSMessageDeliveredEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Delivered;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(SMSMessageRejectedEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Rejected;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        private void PlayEvent(SMSMessageUndeliveredEvent domainEvent)
        {
            this.DeliveryStatusList[this.ResendCount] = MessageStatus.Undeliverable;
        }

        public Int32 ResendCount { get; private set; }
        private List<MessageStatus> DeliveryStatusList;

        public void ResendRequestToProvider()
        {
            if (this.DeliveryStatusList[this.ResendCount] != MessageStatus.Sent &&
                this.DeliveryStatusList[this.ResendCount] != MessageStatus.Delivered)
            {
                throw new InvalidOperationException($"Cannot re-send a message to provider that has not already been sent. Current Status [{this.DeliveryStatusList[this.ResendCount]}]");
            }

            RequestResentToSMSProviderEvent requestResentToSMSProviderEvent = new RequestResentToSMSProviderEvent(this.AggregateId);

            this.ApplyAndAppend(requestResentToSMSProviderEvent);
        }

        public MessageStatus GetDeliveryStatus(Int32? resendAttempt = null)
        {
            if (resendAttempt.HasValue == false)
            {
                return this.DeliveryStatusList[this.ResendCount];
            }
            return this.DeliveryStatusList[resendAttempt.Value];
        }

        private void PlayEvent(RequestResentToSMSProviderEvent domainEvent)
        {
            this.ResendCount++;
            this.DeliveryStatusList.Add(MessageStatus.InProgress);
        }
    }
}
