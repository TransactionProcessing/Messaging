using Xunit;

namespace MessagingService.EmailAggregate.Tests
{
    using System;
    using EmailMessageAggregate;
    using Shouldly;
    using Testing;

    public class EmailAggregateTests
    {
        [Fact]
        public void EmailAggregate_CanBeCreated_IsCreated()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.MessageId.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public void EmailAggregate_SendRequestToProvider_RequestSent()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);

            emailAggregate.FromAddress.ShouldBe(TestData.FromAddress);
            emailAggregate.Subject.ShouldBe(TestData.Subject);
            emailAggregate.Body.ShouldBe(TestData.Body);
            emailAggregate.IsHtml.ShouldBe(TestData.IsHtmlTrue);
            // TODO: Get Recipients
        }

        [Fact]
        public void EmailAggregate_ReceiveResponseFromProvider_ResponseReceived()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.ProviderRequestReference.ShouldBe(TestData.ProviderRequestReference);
            emailAggregate.ProviderEmailReference.ShouldBe(TestData.ProviderEmailReference);
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsDelivered_MessageMarkedAsDelivered()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            emailAggregate.MessageStatus.ShouldBe(MessageStatus.Delivered);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsDelivered_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            
            switch(messageStatus)
            {
                case MessageStatus.Delivered:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Failed:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;

            }

            Should.Throw<InvalidOperationException>(() =>
                                                    {
                                                        emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                                                    });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsRejected_MessageMarkedAsRejected()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);

            emailAggregate.MessageStatus.ShouldBe(MessageStatus.Rejected);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsRejected_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            
            switch (messageStatus)
            {
                case MessageStatus.Delivered:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Failed:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
            });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsFailed_MessageMarkedAsFailed()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);

            emailAggregate.MessageStatus.ShouldBe(MessageStatus.Failed);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsFailed_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);

            switch (messageStatus)
            {
                case MessageStatus.Delivered:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Failed:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
            });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsBounced_MessageMarkedAsBounced()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);

            emailAggregate.MessageStatus.ShouldBe(MessageStatus.Bounced);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsBounced_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);

            switch (messageStatus)
            {
                case MessageStatus.Delivered:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Failed:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
            });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsSpam_MessageMarkedAsSpam()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);

            emailAggregate.MessageStatus.ShouldBe(MessageStatus.Spam);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsSpam_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue);

            switch (messageStatus)
            {
                case MessageStatus.Delivered:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Failed:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
                    emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
            });
        }
    }
}
