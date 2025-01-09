using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.SMSAggregate.Tests
{
    using EmailMessageAggregate;
    using Shared.Logger;
    using Shouldly;
    using SMSMessageAggregate;
    using Testing;
    using Xunit;
    using MessageStatus = SMSMessageAggregate.MessageStatus;

    public class SMSAggregateTests
    {
        [Fact]
        public void SMSAggregate_CanBeCreated_IsCreated()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.MessageId.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public void SMSAggregate_SendRequestToProvider_RequestSent()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.Sender.ShouldBe(TestData.Sender);
            smsAggregate.Destination.ShouldBe(TestData.Destination);
            smsAggregate.Message.ShouldBe(TestData.Message);
            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.InProgress);
        }

        [Fact]
        public void SMSAggregate_ReceiveResponseFromProvider_ResponseReceived()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.ProviderReference.ShouldBe(TestData.ProviderSMSReference);
            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.Sent);
        }

        [Fact]
        public void SMSAggregate_MarkMessageAsDelivered_MessageMarkedAsDelivered()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.Delivered);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Expired)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Undeliverable)]
        [InlineData(MessageStatus.InProgress)]
        [InlineData(MessageStatus.Rejected)]
        public void SMSAggregate_MarkMessageAsDelivered_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);

            switch (messageStatus)
            {
                case MessageStatus.Expired:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Delivered:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Undeliverable:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.InProgress:
                    break;
                case MessageStatus.Rejected:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            });
        }

        [Fact]
        public void SMSAggregate_MarkMessageAsExpired_MessageMarkedAsExpired()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.Expired);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Expired)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Undeliverable)]
        [InlineData(MessageStatus.InProgress)]
        [InlineData(MessageStatus.Rejected)]
        public void SMSAggregate_MarkMessageAsExpired_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);

            switch (messageStatus)
            {
                case MessageStatus.Expired:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Delivered:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Undeliverable:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.InProgress:
                    break;
                case MessageStatus.Rejected:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            });
        }

        [Fact]
        public void SMSAggregate_MarkMessageAsUndeliverable_MessageMarkedAsUndeliverable()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.Undeliverable);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Expired)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Undeliverable)]
        [InlineData(MessageStatus.InProgress)]
        [InlineData(MessageStatus.Rejected)]
        public void SMSAggregate_MarkMessageAsUndeliverable_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);

            switch (messageStatus)
            {
                case MessageStatus.Expired:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Delivered:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Undeliverable:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.InProgress:
                    break;
                case MessageStatus.Rejected:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            });
        }

        [Fact]
        public void SMSAggregate_MarkMessageAsRejected_MessageMarkedAsRejected()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

            smsAggregate.GetDeliveryStatus().ShouldBe(MessageStatus.Rejected);
        }

        [Theory]
        [InlineData(MessageStatus.NotSet)]
        [InlineData(MessageStatus.Expired)]
        [InlineData(MessageStatus.Delivered)]
        [InlineData(MessageStatus.Undeliverable)]
        [InlineData(MessageStatus.InProgress)]
        [InlineData(MessageStatus.Rejected)]
        public void SMSAggregate_MarkMessageAsRejected_IncorrectState_ErrorThrown(MessageStatus messageStatus)
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);

            switch (messageStatus)
            {
                case MessageStatus.Expired:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.BouncedDateTime);
                    break;
                case MessageStatus.Delivered:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
                    break;
                case MessageStatus.Undeliverable:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.InProgress:
                    break;
                case MessageStatus.Rejected:
                    smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
                    smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.FailedDateTime);
                    break;
                case MessageStatus.NotSet:
                    break;
            }

            Should.Throw<InvalidOperationException>(() =>
            {
                smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            });
        }

        [Fact]
        public void SMSAggregate_ResendRequestToProvider_IsSent_MessageIsResent()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.ResendRequestToProvider();

            smsAggregate.ResendCount.ShouldBe(1);
            smsAggregate.GetDeliveryStatus(1).ShouldBe(MessageStatus.InProgress);
        }

        
        [Fact]
        public void SMSAggregate_ResendRequestToProvider_IsDelivered_MessageIsResent()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
            smsAggregate.MarkMessageAsDelivered(TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
            smsAggregate.ResendRequestToProvider();

            smsAggregate.ResendCount.ShouldBe(1);
            smsAggregate.GetDeliveryStatus(1).ShouldBe(MessageStatus.InProgress);
        }
        
        [Fact]
        public void SMSAggregate_ResendRequestToProvider_NotSet_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            Should.Throw<InvalidOperationException>(() => smsAggregate.ResendRequestToProvider());
        }
        
        [Fact]
        public void SMSAggregate_ResendRequestToProvider_InProgress_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            Should.Throw<InvalidOperationException>(() => smsAggregate.ResendRequestToProvider());
        }
        
        [Fact]
        public void SMSAggregate_ResendRequestToProvider_Rejected_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
            smsAggregate.MarkMessageAsRejected(TestData.ProviderStatusDescription, TestData.RejectedDateTime);
            Should.Throw<InvalidOperationException>(() => smsAggregate.ResendRequestToProvider());
        }
        
        [Fact]
        public void SMSAggregate_ResendRequestToProvider_Expired_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
            smsAggregate.MarkMessageAsExpired(TestData.ProviderStatusDescription, TestData.ExpiredDateTime);
            Should.Throw<InvalidOperationException>(() => smsAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void SMSAggregate_ResendRequestToProvider_Undelivered_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
            smsAggregate.MarkMessageAsUndeliverable(TestData.ProviderStatusDescription, TestData.UndeliveredDateTime);
            Should.Throw<InvalidOperationException>(() => smsAggregate.ResendRequestToProvider());
        }

        [Fact]
        public void SMSAggregate_PlayEvent_UnsupportedEvent_ErrorThrown()
        {
            Logger.Initialise(NullLogger.Instance);
            SMSAggregate smsAggregate = new SMSAggregate();
            Should.Throw<Exception>(() => smsAggregate.PlayEvent(new TestEvent(Guid.NewGuid(), Guid.NewGuid())));
        }
    }
}