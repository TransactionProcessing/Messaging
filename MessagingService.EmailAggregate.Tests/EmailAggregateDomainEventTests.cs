namespace MessagingService.EmailAggregate.Tests
{
    using System;
    using EmailMessage.DomainEvents;
    using Shouldly;
    using Testing;
    using Xunit;

    public class EmailAggregateDomainEventTests
    {
        [Fact]
        public void RequestSentToProviderEvent_CanBeCreated_IsCreated()
        {
            RequestSentToProviderEvent requestSentToProviderEvent = RequestSentToProviderEvent.Create(TestData.MessageId, TestData.FromAddress, TestData.ToAddresses, TestData.Subject,
                                                                                                      TestData.Body,TestData.IsHtmlTrue);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.FromAddress.ShouldBe(TestData.FromAddress);
            requestSentToProviderEvent.ToAddresses.ShouldBe(TestData.ToAddresses);
            requestSentToProviderEvent.Subject.ShouldBe(TestData.Subject);
            requestSentToProviderEvent.Body.ShouldBe(TestData.Body);
            requestSentToProviderEvent.IsHtml.ShouldBe(TestData.IsHtmlTrue);

        }

        [Fact]
        public void ResponseReceivedFromProviderEvent_CanBeCreated_IsCreated()
        {
            ResponseReceivedFromProviderEvent requestSentToProviderEvent = ResponseReceivedFromProviderEvent.Create(TestData.MessageId, TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.ProviderRequestReference.ShouldBe(TestData.ProviderRequestReference);
            requestSentToProviderEvent.ProviderEmailReference.ShouldBe(TestData.ProviderEmailReference);
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
        public void MessageFailedEvent_CanBeCreated_IsCreated()
        {
            MessageFailedEvent messageFailedEvent =
                MessageFailedEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.FailedDateTime);

            messageFailedEvent.ShouldNotBeNull();
            messageFailedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageFailedEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageFailedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageFailedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageFailedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageFailedEvent.FailedDateTime.ShouldBe(TestData.FailedDateTime);
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
            MessageBouncedEvent messageBouncedEvent =
                MessageBouncedEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.BouncedDateTime);

            messageBouncedEvent.ShouldNotBeNull();
            messageBouncedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageBouncedEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageBouncedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageBouncedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageBouncedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageBouncedEvent.BouncedDateTime.ShouldBe(TestData.BouncedDateTime);
        }

        [Fact]
        public void MessageMarkedAsSpamEvent_CanBeCreated_IsCreated()
        {
            MessageMarkedAsSpamEvent messageMarkedAsSpamEvent =
                MessageMarkedAsSpamEvent.Create(TestData.MessageId, TestData.ProviderStatusDescription, TestData.SpamDateTime);

            messageMarkedAsSpamEvent.ShouldNotBeNull();
            messageMarkedAsSpamEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageMarkedAsSpamEvent.EventCreatedDateTime.ShouldNotBe(DateTime.MinValue);
            messageMarkedAsSpamEvent.EventId.ShouldNotBe(Guid.Empty);
            messageMarkedAsSpamEvent.MessageId.ShouldBe(TestData.MessageId);
            messageMarkedAsSpamEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageMarkedAsSpamEvent.SpamDateTime.ShouldBe(TestData.SpamDateTime);
        }
    }
}