using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.SMSAggregate.Tests
{
    using EmailMessageAggregate;
    using Shouldly;
    using SMSMessageAggregate;
    using Testing;
    using Xunit;
    using MessageStatus = SMSMessageAggregate.MessageStatus;

    public class EmailAggregateTests
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
            smsAggregate.MessageStatus.ShouldBe(MessageStatus.InProgress);
        }

        [Fact]
        public void SMSAggregate_SendRequestToProvider_RequestAlreadySent_ErrorThrown()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);

            Should.Throw<InvalidOperationException>(() =>
            {
                smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            });
        }

        [Fact]
        public void SMSAggregate_ReceiveResponseFromProvider_ResponseReceived()
        {
            SMSAggregate smsAggregate = SMSAggregate.Create(TestData.MessageId);

            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);

            smsAggregate.ProviderReference.ShouldBe(TestData.ProviderSMSReference);
            smsAggregate.MessageStatus.ShouldBe(MessageStatus.Sent);
        }
    }
}
