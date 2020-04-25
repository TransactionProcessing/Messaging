using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.Requests
{
    using BusinessLogic.Requests;
    using Shouldly;
    using Testing;
    using Xunit;

    public class RequestTests
    {
        [Fact]
        public void SendEmailRequest_CanBeCreated_IsCreated()
        {
            SendEmailRequest request = SendEmailRequest.Create(TestData.ConnectionIdentifier,
                                                               TestData.MessageId,
                                                               TestData.FromAddress,
                                                               TestData.ToAddresses,
                                                               TestData.Subject,
                                                               TestData.Body,
                                                               TestData.IsHtmlTrue);

            request.ShouldNotBeNull();
            request.ConnectionIdentifier.ShouldBe(TestData.ConnectionIdentifier);
            request.MessageId.ShouldBe(TestData.MessageId);
            request.FromAddress.ShouldBe(TestData.FromAddress);
            request.ToAddresses.ShouldBe(TestData.ToAddresses);
            request.Subject.ShouldBe(TestData.Subject);
            request.Body.ShouldBe(TestData.Body);
            request.IsHtml.ShouldBe(TestData.IsHtmlTrue);
        }
    }
}
