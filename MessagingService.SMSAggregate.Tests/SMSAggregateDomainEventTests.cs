using System;
using Xunit;

namespace MessagingService.SMSAggregate.Tests
{
    using Shouldly;
    using SMSMessage.DomainEvents;
    using Testing;

    public class SMSAggregateDomainEventTests
    {
        [Fact]
        public void RequestSentToSMSProviderEvent_CanBeCreated_IsCreated()
        {
            RequestSentToSMSProviderEvent requestSentToProviderEvent = new RequestSentToSMSProviderEvent(TestData.MessageId, TestData.Sender, TestData.Destination, TestData.Message);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.Sender.ShouldBe(TestData.Sender);
            requestSentToProviderEvent.Destination.ShouldBe(TestData.Destination);
            requestSentToProviderEvent.Message.ShouldBe(TestData.Message);
        }

        [Fact]
        public void ResponseReceivedFromSMSProviderEvent_CanBeCreated_IsCreated()
        {
            ResponseReceivedFromSMSProviderEvent requestSentToProviderEvent = new ResponseReceivedFromSMSProviderEvent(TestData.MessageId, TestData.ProviderSMSReference);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.ProviderSMSReference.ShouldBe(TestData.ProviderSMSReference);
        }

        [Fact]
        public void SMSMessageDeliveredEvent_CanBeCreated_IsCreated()
        {
            SMSMessageDeliveredEvent messageDeliveredEvent =
                new SMSMessageDeliveredEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            messageDeliveredEvent.ShouldNotBeNull();
            messageDeliveredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageDeliveredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageDeliveredEvent.DeliveredDateTime.ShouldBe(TestData.DeliveredDateTime);
        }

        [Fact]
        public void SMSMessageUndeliveredEvent_CanBeCreated_IsCreated()
        {
            SMSMessageUndeliveredEvent messageUndeliveredEvent =
                new SMSMessageUndeliveredEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.UndeliveredDateTime);

            messageUndeliveredEvent.ShouldNotBeNull();
            messageUndeliveredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageUndeliveredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageUndeliveredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageUndeliveredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageUndeliveredEvent.UndeliveredDateTime.ShouldBe(TestData.UndeliveredDateTime);
        }

        [Fact]
        public void SMSMessageRejectedEvent_CanBeCreated_IsCreated()
        {
            SMSMessageRejectedEvent messageRejectedEvent =
                new SMSMessageRejectedEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.RejectedDateTime);

            messageRejectedEvent.ShouldNotBeNull();
            messageRejectedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageRejectedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageRejectedEvent.RejectedDateTime.ShouldBe(TestData.RejectedDateTime);
        }

        [Fact]
        public void SMSMessageBouncedEvent_CanBeCreated_IsCreated()
        {
            SMSMessageExpiredEvent messageExpiredEvent =
                new SMSMessageExpiredEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.ExpiredDateTime);

            messageExpiredEvent.ShouldNotBeNull();
            messageExpiredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageExpiredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageExpiredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageExpiredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageExpiredEvent.ExpiredDateTime.ShouldBe(TestData.ExpiredDateTime);
        }

    }
}
