namespace MessagingService.SMSMessageAggregate{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.General;
    using SMSMessage.DomainEvents;

    public static class SMSAggregateExtensions{
        #region Methods

        public static MessageStatus GetDeliveryStatus(this SMSAggregate aggregate, Int32? resendAttempt = null){
            if (resendAttempt.HasValue == false){
                return aggregate.DeliveryStatusList[aggregate.ResendCount];
            }

            return aggregate.DeliveryStatusList[resendAttempt.Value];
        }

        public static void MarkMessageAsDelivered(this SMSAggregate aggregate,
                                                  String providerStatus,
                                                  DateTime failedDateTime){
            aggregate.CheckMessageCanBeSetToDelivered();

            SMSMessageDeliveredEvent messageDeliveredEvent = new SMSMessageDeliveredEvent(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageDeliveredEvent);
        }

        public static void MarkMessageAsExpired(this SMSAggregate aggregate,
                                                String providerStatus,
                                                DateTime failedDateTime){
            aggregate.CheckMessageCanBeSetToExpired();

            SMSMessageExpiredEvent messageExpiredEvent = new SMSMessageExpiredEvent(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageExpiredEvent);
        }

        public static void MarkMessageAsRejected(this SMSAggregate aggregate,
                                                 String providerStatus,
                                                 DateTime failedDateTime){
            aggregate.CheckMessageCanBeSetToRejected();

            SMSMessageRejectedEvent messageRejectedEvent = new SMSMessageRejectedEvent(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageRejectedEvent);
        }

        public static void MarkMessageAsUndeliverable(this SMSAggregate aggregate,
                                                      String providerStatus,
                                                      DateTime failedDateTime){
            aggregate.CheckMessageCanBeSetToUndeliverable();

            SMSMessageUndeliveredEvent messageUndeliveredEvent = new SMSMessageUndeliveredEvent(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageUndeliveredEvent);
        }

        public static void PlayEvent(this SMSAggregate aggregate, RequestSentToSMSProviderEvent domainEvent){
            aggregate.Sender = domainEvent.Sender;
            aggregate.Destination = domainEvent.Destination;
            aggregate.Message = domainEvent.Message;
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.InProgress;
        }

        public static void PlayEvent(this SMSAggregate aggregate, ResponseReceivedFromSMSProviderEvent domainEvent){
            aggregate.ProviderReference = domainEvent.ProviderSMSReference;
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Sent;
        }

        public static void PlayEvent(this SMSAggregate aggregate, SMSMessageExpiredEvent domainEvent){
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Expired;
        }

        public static void PlayEvent(this SMSAggregate aggregate, SMSMessageDeliveredEvent domainEvent){
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Delivered;
        }

        public static void PlayEvent(this SMSAggregate aggregate, SMSMessageRejectedEvent domainEvent){
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Rejected;
        }

        public static void PlayEvent(this SMSAggregate aggregate, SMSMessageUndeliveredEvent domainEvent){
            aggregate.DeliveryStatusList[aggregate.ResendCount] = MessageStatus.Undeliverable;
        }

        public static void PlayEvent(this SMSAggregate aggregate, RequestResentToSMSProviderEvent domainEvent){
            aggregate.ResendCount++;
            aggregate.DeliveryStatusList.Add(MessageStatus.InProgress);
        }

        public static void ReceiveResponseFromProvider(this SMSAggregate aggregate, String providerSMSReference){
            ResponseReceivedFromSMSProviderEvent responseReceivedFromProviderEvent =
                new ResponseReceivedFromSMSProviderEvent(aggregate.AggregateId, providerSMSReference);

            aggregate.ApplyAndAppend(responseReceivedFromProviderEvent);
        }

        public static void ResendRequestToProvider(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent &&
                aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Delivered){
                throw new InvalidOperationException($"Cannot re-send a message to provider that has not already been sent. Current Status [{aggregate.DeliveryStatusList[aggregate.ResendCount]}]");
            }

            RequestResentToSMSProviderEvent requestResentToSMSProviderEvent = new RequestResentToSMSProviderEvent(aggregate.AggregateId);

            aggregate.ApplyAndAppend(requestResentToSMSProviderEvent);
        }

        public static void SendRequestToProvider(this SMSAggregate aggregate,
                                                 String sender,
                                                 String destination,
                                                 String message){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.NotSet){
                return;
            }

            RequestSentToSMSProviderEvent requestSentToProviderEvent = new RequestSentToSMSProviderEvent(aggregate.AggregateId, sender, destination, message);

            aggregate.ApplyAndAppend(requestSentToProviderEvent);
        }

        private static void CheckMessageCanBeSetToDelivered(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to delivered");
            }
        }

        private static void CheckMessageCanBeSetToExpired(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to expired");
            }
        }

        private static void CheckMessageCanBeSetToRejected(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to rejected");
            }
        }

        private static void CheckMessageCanBeSetToUndeliverable(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                throw new InvalidOperationException($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to undeliverable");
            }
        }

        #endregion
    }

    public record SMSAggregate : Aggregate{
        #region Fields

        internal List<MessageStatus> DeliveryStatusList;

        #endregion

        #region Constructors

        [ExcludeFromCodeCoverage]
        public SMSAggregate(){
            this.DeliveryStatusList = new List<MessageStatus>{
                                                                 MessageStatus.NotSet
                                                             };
        }

        private SMSAggregate(Guid aggregateId){
            Guard.ThrowIfInvalidGuid(aggregateId, "Aggregate Id cannot be an Empty Guid");

            this.AggregateId = aggregateId;
            this.MessageId = aggregateId;
            this.DeliveryStatusList = new List<MessageStatus>{
                                                                 MessageStatus.NotSet
                                                             };
        }

        #endregion

        #region Properties

        public String Destination{ get; internal set; }

        public String Message{ get; internal set; }

        public Guid MessageId{ get; internal set; }

        public String ProviderReference{ get; internal set; }

        public Int32 ResendCount{ get; internal set; }

        public String Sender{ get; internal set; }

        #endregion

        #region Methods

        public static SMSAggregate Create(Guid aggregateId){
            return new SMSAggregate(aggregateId);
        }

        public override void PlayEvent(IDomainEvent domainEvent) => SMSAggregateExtensions.PlayEvent(this, (dynamic)domainEvent);

        [ExcludeFromCodeCoverage]
        protected override Object GetMetadata(){
            return null;
        }

        #endregion
    }
}