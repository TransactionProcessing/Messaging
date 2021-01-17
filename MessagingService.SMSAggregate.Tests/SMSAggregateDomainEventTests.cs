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
        public void RequestSentToProviderEvent_CanBeCreated_IsCreated()
        {
            RequestSentToProviderEvent requestSentToProviderEvent = RequestSentToProviderEvent.Create(TestData.MessageId, TestData.Sender, TestData.Destination, TestData.Message);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.Sender.ShouldBe(TestData.Sender);
            requestSentToProviderEvent.Destination.ShouldBe(TestData.Destination);
            requestSentToProviderEvent.Message.ShouldBe(TestData.Message);
        }

        [Fact]
        public void ResponseReceivedFromProviderEvent_CanBeCreated_IsCreated()
        {
            ResponseReceivedFromProviderEvent requestSentToProviderEvent = ResponseReceivedFromProviderEvent.Create(TestData.MessageId, TestData.ProviderSMSReference);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.ProviderSMSReference.ShouldBe(TestData.ProviderSMSReference);
        }

        [Fact]
        public void MessageDeliveredEvent_CanBeCreated_IsCreated()
        {
            MessageDeliveredEvent messageDeliveredEvent =
                MessageDeliveredEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            messageDeliveredEvent.ShouldNotBeNull();
            messageDeliveredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageDeliveredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageDeliveredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageDeliveredEvent.DeliveredDateTime.ShouldBe(TestData.DeliveredDateTime);
        }

        [Fact]
        public void MessageUndeliveredEvent_CanBeCreated_IsCreated()
        {
            MessageUndeliveredEvent messageUndeliveredEvent=
                MessageUndeliveredEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.UndeliveredDateTime);

            messageUndeliveredEvent.ShouldNotBeNull();
            messageUndeliveredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageUndeliveredEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageUndeliveredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageUndeliveredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageUndeliveredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageUndeliveredEvent.UndeliveredDateTime.ShouldBe(TestData.UndeliveredDateTime);
        }

        [Fact]
        public void MessageRejectedEvent_CanBeCreated_IsCreated()
        {
            MessageRejectedEvent messageRejectedEvent =
                MessageRejectedEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.RejectedDateTime);

            messageRejectedEvent.ShouldNotBeNull();
            messageRejectedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageRejectedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageRejectedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageRejectedEvent.RejectedDateTime.ShouldBe(TestData.RejectedDateTime);
        }

        [Fact]
        public void MessageBouncedEvent_CanBeCreated_IsCreated()
        {
            MessageExpiredEvent messageExpiredEvent=
                MessageExpiredEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.ExpiredDateTime);

            messageExpiredEvent.ShouldNotBeNull();
            messageExpiredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageExpiredEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageExpiredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageExpiredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageExpiredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageExpiredEvent.ExpiredDateTime.ShouldBe(TestData.ExpiredDateTime);
        }

    }
}
