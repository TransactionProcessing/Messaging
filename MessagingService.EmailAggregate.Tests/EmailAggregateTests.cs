using System.Linq;
using Xunit;

namespace MessagingService.EmailAggregate.Tests
{
    using System;
    using System.Collections.Generic;
    using EmailMessageAggregate;
    using Models;
    using Shared.Logger;
    using Shouldly;
    using Testing;

    public class EmailAggregateTests
    {
        [Fact]
        public void EmailAggregate_CanBeCreated_IsCreated() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.MessageId.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public void EmailAggregate_SendRequestToProvider_RequestSent() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            emailAggregate.FromAddress.ShouldBe(TestData.FromAddress);
            emailAggregate.Subject.ShouldBe(TestData.Subject);
            emailAggregate.Body.ShouldBe(TestData.Body);
            emailAggregate.IsHtml.ShouldBe(TestData.IsHtmlTrue);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.InProgress);
            var toAddresses = emailAggregate.GetToAddresses();
            toAddresses.Count.ShouldBe(TestData.ToAddresses.Count);
            List<EmailAttachment> attachments = emailAggregate.GetAttachments();
            attachments.Count.ShouldBe(TestData.EmailAttachmentModels.Count);
            var recipients = emailAggregate.GetRecipients();
            recipients.Count.ShouldBe(TestData.ToAddresses.Count);
            foreach (var toAddress in TestData.ToAddresses) {
                var r = recipients.SingleOrDefault(r => r.ToAddress == toAddress);
                r.ShouldNotBeNull();
            }
        }

        [Fact]
        public void EmailAggregate_ReceiveResponseFromProvider_ResponseReceived() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.ProviderRequestReference.ShouldBe(TestData.ProviderRequestReference);
            emailAggregate.ProviderEmailReference.ShouldBe(TestData.ProviderEmailReference);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Sent);
        }

        [Fact]
        public void EmailAggregate_ReceiveBadResponseFromProvider_ResponseReceived()
        {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveBadResponseFromProvider(TestData.EmailError, TestData.EmailErrorCode);

            emailAggregate.Error.ShouldBe(TestData.EmailError);
            emailAggregate.ErrorCode.ShouldBe(TestData.EmailErrorCode);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Failed);
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsDelivered_MessageMarkedAsDelivered() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Delivered);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsDelivered_IncorrectState_ErrorThrown(MessageStatus messageStatus) {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            switch(messageStatus) {
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

            Should.Throw<InvalidOperationException>(() => { emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime); });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsRejected_MessageMarkedAsRejected() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Rejected);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsRejected_IncorrectState_ErrorThrown(MessageStatus messageStatus) {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            switch(messageStatus) {
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

            Should.Throw<InvalidOperationException>(() => { emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime); });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsFailed_MessageMarkedAsFailed() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Failed);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Spam)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsFailed_IncorrectState_ErrorThrown(MessageStatus messageStatus) {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            switch(messageStatus) {
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

            Should.Throw<InvalidOperationException>(() => { emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime); });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsBounced_MessageMarkedAsBounced() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Bounced);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Spam)]
        public void EmailAggregate_MarkMessageAsBounced_IncorrectState_ErrorThrown(MessageStatus messageStatus) {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            switch(messageStatus) {
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

            Should.Throw<InvalidOperationException>(() => { emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime); });
        }

        [Fact]
        public void EmailAggregate_MarkMessageAsSpam_MessageMarkedAsSpam() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);

            emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
            MessageStatus messageStatus = emailAggregate.GetDeliveryStatus();
            messageStatus.ShouldBe(MessageStatus.Spam);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Rejected)]
        [InlineData(MessageStatus.Failed)]
        [InlineData(MessageStatus.Bounced)]
        public void EmailAggregate_MarkMessageAsSpam_IncorrectState_ErrorThrown(MessageStatus messageStatus) {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);

            switch(messageStatus) {
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

            Should.Throw<InvalidOperationException>(() => { emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime); });
        }



        [Fact]
        public void EmailAggregate_ResendRequestToProvider_IsSent_MessageIsResent() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.ResendRequestToProvider();

            emailAggregate.ResendCount.ShouldBe(1);
            emailAggregate.GetDeliveryStatus(1).ShouldBe(MessageStatus.InProgress);
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_IsDelivered_MessageIsResent() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            emailAggregate.ResendRequestToProvider();

            emailAggregate.ResendCount.ShouldBe(1);
            emailAggregate.GetDeliveryStatus(1).ShouldBe(MessageStatus.InProgress);
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_NotSet_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_InProgress_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_Rejected_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_Failed_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.MarkMessageAsFailed(TestData.ProviderStatusDescription, TestData.FailedDateTime);
            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_Spam_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.MarkMessageAsSpam(TestData.ProviderStatusDescription, TestData.SpamDateTime);
            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_ResendRequestToProvider_Bounced_ErrorThrown() {
            EmailAggregate emailAggregate = EmailAggregate.Create(TestData.MessageId);

            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject, TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            emailAggregate.MarkMessageAsBounced(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
            Should.Throw<InvalidOperationException>(() => emailAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void EmailAggregate_PlayEvent_UnsupportedEvent_ErrorThrown() {
            Logger.Initialise(NullLogger.Instance);
            EmailAggregate emailAggregate = new EmailAggregate();
            Should.Throw<Exception>(() => emailAggregate.PlayEvent(new TestEvent(Guid.NewGuid(), Guid.NewGuid())));
        }
    }
}