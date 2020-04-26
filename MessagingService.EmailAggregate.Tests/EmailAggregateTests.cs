using Xunit;

namespace MessagingService.EmailAggregate.Tests
{
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
    }
}
