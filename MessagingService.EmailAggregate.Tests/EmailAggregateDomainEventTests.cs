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
    }
}