using SimpleResults;

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

        public static Result MarkMessageAsDelivered(this SMSAggregate aggregate,
                                                  String providerStatus,
                                                  DateTime failedDateTime){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] == MessageStatus.Delivered)
                return Result.Success();

            Result result = aggregate.CheckMessageCanBeSetToDelivered();
            if (result.IsFailed)
                return result;
            SMSMessageDeliveredEvent messageDeliveredEvent = new(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageDeliveredEvent);
            return Result.Success();
        }

        public static Result MarkMessageAsExpired(this SMSAggregate aggregate,
                                                String providerStatus,
                                                DateTime failedDateTime){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] == MessageStatus.Expired)
                return Result.Success();
            Result result = aggregate.CheckMessageCanBeSetToExpired();
            if (result.IsFailed)
                return result;
            SMSMessageExpiredEvent messageExpiredEvent = new(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageExpiredEvent);

            return Result.Success();
        }

        public static Result MarkMessageAsRejected(this SMSAggregate aggregate,
                                                 String providerStatus,
                                                 DateTime failedDateTime){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] == MessageStatus.Rejected)
                return Result.Success();
            Result result = aggregate.CheckMessageCanBeSetToRejected();
            if (result.IsFailed)
                return result;
            SMSMessageRejectedEvent messageRejectedEvent = new(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageRejectedEvent);

            return Result.Success();
        }

        public static Result MarkMessageAsUndeliverable(this SMSAggregate aggregate,
                                                      String providerStatus,
                                                      DateTime failedDateTime){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] == MessageStatus.Undeliverable)
                return Result.Success();
            Result result = aggregate.CheckMessageCanBeSetToUndeliverable();
            if (result.IsFailed)
                return result;
            SMSMessageUndeliveredEvent messageUndeliveredEvent = new(aggregate.AggregateId, providerStatus, failedDateTime);

            aggregate.ApplyAndAppend(messageUndeliveredEvent);
            return Result.Success();
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

        public static Result ReceiveResponseFromProvider(this SMSAggregate aggregate, String providerSMSReference){
            ResponseReceivedFromSMSProviderEvent responseReceivedFromProviderEvent =
                new(aggregate.AggregateId, providerSMSReference);

            aggregate.ApplyAndAppend(responseReceivedFromProviderEvent);

            return Result.Success();
        }

        public static Result ResendRequestToProvider(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent &&
                aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Delivered){
                return Result.Invalid($"Cannot re-send a message to provider that has not already been sent. Current Status [{aggregate.DeliveryStatusList[aggregate.ResendCount]}]");
            }

            RequestResentToSMSProviderEvent requestResentToSMSProviderEvent = new(aggregate.AggregateId);

            aggregate.ApplyAndAppend(requestResentToSMSProviderEvent);

            return Result.Success();
        }

        public static Result SendRequestToProvider(this SMSAggregate aggregate,
                                                 String sender,
                                                 String destination,
                                                 String message){
            

            RequestSentToSMSProviderEvent requestSentToProviderEvent = new(aggregate.AggregateId, sender, destination, message);

            aggregate.ApplyAndAppend(requestSentToProviderEvent);

            return Result.Success();
        }

        private static Result CheckMessageCanBeSetToDelivered(this SMSAggregate aggregate) {
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                return Result.Invalid($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to delivered");
            }
            return Result.Success();
        }

        private static Result CheckMessageCanBeSetToExpired(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                return Result.Invalid($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to expired");
            }
            return Result.Success();
        }

        private static Result CheckMessageCanBeSetToRejected(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                return Result.Invalid($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to rejected");
            }
            return Result.Success();
        }

        private static Result CheckMessageCanBeSetToUndeliverable(this SMSAggregate aggregate){
            if (aggregate.DeliveryStatusList[aggregate.ResendCount] != MessageStatus.Sent){
                return Result.Invalid($"Message at status {aggregate.DeliveryStatusList[aggregate.ResendCount]} cannot be set to undeliverable");
            }
            return Result.Success();
        }

        public static MessageStatus GetMessageStatus(this SMSAggregate aggregate) {
            return aggregate.DeliveryStatusList[aggregate.ResendCount];
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
            if (aggregateId == Guid.Empty)
                throw new ArgumentNullException(nameof(aggregateId));

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