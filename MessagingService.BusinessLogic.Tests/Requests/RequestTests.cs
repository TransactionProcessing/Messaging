﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.Requests
{
    using BusinessLogic.Requests;
    using Shouldly;
    using System.Linq;
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
                                                               TestData.IsHtmlTrue,
                                                               TestData.EmailAttachments);

            request.ShouldNotBeNull();
            request.ConnectionIdentifier.ShouldBe(TestData.ConnectionIdentifier);
            request.MessageId.ShouldBe(TestData.MessageId);
            request.FromAddress.ShouldBe(TestData.FromAddress);
            request.ToAddresses.ShouldBe(TestData.ToAddresses);
            request.Subject.ShouldBe(TestData.Subject);
            request.Body.ShouldBe(TestData.Body);
            request.IsHtml.ShouldBe(TestData.IsHtmlTrue);
            request.EmailAttachments.Count.ShouldBe(TestData.EmailAttachments.Count);
            request.EmailAttachments[0].FileData.ShouldBe(TestData.FileData);
            request.EmailAttachments[0].Filename.ShouldBe(TestData.FileName);
            request.EmailAttachments[0].FileType.ShouldBe(TestData.FileTypePDF);
            request.EmailAttachments[1].FileData.ShouldBe(TestData.FileData);
            request.EmailAttachments[1].Filename.ShouldBe(TestData.FileName);
            request.EmailAttachments[1].FileType.ShouldBe(TestData.FileTypeNone);

        }

        [Fact]
        public void SendSMSRequest_CanBeCreated_IsCreated()
        {
            SendSMSRequest request = SendSMSRequest.Create(TestData.ConnectionIdentifier,
                                                           TestData.MessageId,
                                                           TestData.Sender,
                                                           TestData.Destination,
                                                           TestData.Message);

            request.ShouldNotBeNull();
            request.ConnectionIdentifier.ShouldBe(TestData.ConnectionIdentifier);
            request.MessageId.ShouldBe(TestData.MessageId);
            request.Sender.ShouldBe(TestData.Sender);
            request.Destination.ShouldBe(TestData.Destination);
            request.Message.ShouldBe(TestData.Message);
        }

        [Fact]
        public void ResendEmailRequest_CanBeCreated_IsCreated() {
            ResendEmailRequest request = ResendEmailRequest.Create(TestData.ConnectionIdentifier, TestData.MessageId);
            request.ShouldNotBeNull();
            request.ConnectionIdentifier.ShouldBe(TestData.ConnectionIdentifier);
            request.MessageId.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public void ResendSMSRequest_CanBeCreated_IsCreated()
        {
            ResendSMSRequest request = ResendSMSRequest.Create(TestData.ConnectionIdentifier, TestData.MessageId);
            request.ShouldNotBeNull();
            request.ConnectionIdentifier.ShouldBe(TestData.ConnectionIdentifier);
            request.MessageId.ShouldBe(TestData.MessageId);
        }
    }
}
