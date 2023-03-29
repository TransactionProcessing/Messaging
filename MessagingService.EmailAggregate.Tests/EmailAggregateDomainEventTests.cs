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
        public void RequestSentToEmailProviderEvent_CanBeCreated_IsCreated()
        {
            RequestSentToEmailProviderEvent requestSentToProviderEvent = new RequestSentToEmailProviderEvent(TestData.MessageId, TestData.FromAddress, TestData.ToAddresses, TestData.Subject,
                                                                                                      TestData.Body,TestData.IsHtmlTrue);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.FromAddress.ShouldBe(TestData.FromAddress);
            requestSentToProviderEvent.ToAddresses.ShouldBe(TestData.ToAddresses);
            requestSentToProviderEvent.Subject.ShouldBe(TestData.Subject);
            requestSentToProviderEvent.Body.ShouldBe(TestData.Body);
            requestSentToProviderEvent.IsHtml.ShouldBe(TestData.IsHtmlTrue);
        }

        [Fact]
        public void EmailAttachmentRequestSentToProviderEvent_CanBeCreated_IsCreated()
        {
            EmailAttachmentRequestSentToProviderEvent emailAttachmentRequestSentToProviderEvent = new EmailAttachmentRequestSentToProviderEvent(TestData.MessageId, TestData.FileName, TestData.FileData,
                                                                                                                                                (Int32)TestData.FileTypePDF);

            emailAttachmentRequestSentToProviderEvent.ShouldNotBeNull();
            emailAttachmentRequestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            emailAttachmentRequestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            emailAttachmentRequestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            emailAttachmentRequestSentToProviderEvent.FileType.ShouldBe((Int32)TestData.FileTypePDF);
            emailAttachmentRequestSentToProviderEvent.Filename.ShouldBe(TestData.FileName);
            emailAttachmentRequestSentToProviderEvent.FileData.ShouldBe(TestData.FileData);
            
        }

        [Fact]
        public void ResponseReceivedFromEmailProviderEvent_CanBeCreated_IsCreated()
        {
            ResponseReceivedFromEmailProviderEvent requestSentToProviderEvent = new ResponseReceivedFromEmailProviderEvent(TestData.MessageId, TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            requestSentToProviderEvent.ShouldNotBeNull();
            requestSentToProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestSentToProviderEvent.MessageId.ShouldBe(TestData.MessageId);
            requestSentToProviderEvent.ProviderRequestReference.ShouldBe(TestData.ProviderRequestReference);
            requestSentToProviderEvent.ProviderEmailReference.ShouldBe(TestData.ProviderEmailReference);
        }

        [Fact]
        public void EmailMessageDeliveredEvent_CanBeCreated_IsCreated()
        {
            EmailMessageDeliveredEvent messageDeliveredEvent =
                new EmailMessageDeliveredEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            messageDeliveredEvent.ShouldNotBeNull();
            messageDeliveredEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.EventId.ShouldNotBe(Guid.Empty);
            messageDeliveredEvent.MessageId.ShouldBe(TestData.MessageId);
            messageDeliveredEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageDeliveredEvent.DeliveredDateTime.ShouldBe(TestData.DeliveredDateTime);
        }

        [Fact]
        public void EmailMessageFailedEvent_CanBeCreated_IsCreated()
        {
            EmailMessageFailedEvent messageFailedEvent =
                new EmailMessageFailedEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.FailedDateTime);

            messageFailedEvent.ShouldNotBeNull();
            messageFailedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageFailedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageFailedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageFailedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageFailedEvent.FailedDateTime.ShouldBe(TestData.FailedDateTime);
        }

        [Fact]
        public void EmailMessageRejectedEvent_CanBeCreated_IsCreated()
        {
            EmailMessageRejectedEvent messageRejectedEvent =
                new EmailMessageRejectedEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.RejectedDateTime);

            messageRejectedEvent.ShouldNotBeNull();
            messageRejectedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageRejectedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageRejectedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageRejectedEvent.RejectedDateTime.ShouldBe(TestData.RejectedDateTime);
        }

        [Fact]
        public void EmailMessageBouncedEvent_CanBeCreated_IsCreated()
        {
            EmailMessageBouncedEvent messageBouncedEvent =
                new EmailMessageBouncedEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.BouncedDateTime);

            messageBouncedEvent.ShouldNotBeNull();
            messageBouncedEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageBouncedEvent.EventId.ShouldNotBe(Guid.Empty);
            messageBouncedEvent.MessageId.ShouldBe(TestData.MessageId);
            messageBouncedEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageBouncedEvent.BouncedDateTime.ShouldBe(TestData.BouncedDateTime);
        }

        [Fact]
        public void EmailMessageMarkedAsSpamEvent_CanBeCreated_IsCreated()
        {
            EmailMessageMarkedAsSpamEvent messageMarkedAsSpamEvent =
                new EmailMessageMarkedAsSpamEvent(TestData.MessageId, TestData.ProviderStatusDescription, TestData.SpamDateTime);

            messageMarkedAsSpamEvent.ShouldNotBeNull();
            messageMarkedAsSpamEvent.AggregateId.ShouldBe(TestData.MessageId);
            messageMarkedAsSpamEvent.EventId.ShouldNotBe(Guid.Empty);
            messageMarkedAsSpamEvent.MessageId.ShouldBe(TestData.MessageId);
            messageMarkedAsSpamEvent.ProviderStatus.ShouldBe(TestData.ProviderStatusDescription);
            messageMarkedAsSpamEvent.SpamDateTime.ShouldBe(TestData.SpamDateTime);
        }

        [Fact]
        public void RequestResentToEmailProviderEvent_CanBeCreated_IsCreated() {
            RequestResentToEmailProviderEvent requestResentToEmailProviderEvent = new RequestResentToEmailProviderEvent(TestData.MessageId);

            requestResentToEmailProviderEvent.ShouldNotBeNull();
            requestResentToEmailProviderEvent.AggregateId.ShouldBe(TestData.MessageId);
            requestResentToEmailProviderEvent.EventId.ShouldNotBe(Guid.Empty);
            requestResentToEmailProviderEvent.MessageId.ShouldBe(TestData.MessageId);
        }
    }
}