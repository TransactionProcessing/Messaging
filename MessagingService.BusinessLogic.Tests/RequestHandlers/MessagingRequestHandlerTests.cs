using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.RequestHandlers
{
    using System.Threading;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using Moq;
    using Services;
    using Shouldly;
    using Testing;
    using Xunit;

    public class MessagingRequestHandlerTests
    {
        [Fact]
        public void MessagingRequestHandler_SendEmailRequest_IsHandled()
        {
            Mock<IMessagingDomainService> messagingDomainService = new Mock<IMessagingDomainService>();
            MessagingRequestHandler handler = new MessagingRequestHandler(messagingDomainService.Object);

            SendEmailRequest command = TestData.SendEmailRequestWithAttachments;

            Should.NotThrow(async () =>
                            {
                                await handler.Handle(command, CancellationToken.None);
                            });

        }

        [Fact]
        public void MessagingRequestHandler_ResendEmailRequest_IsHandled()
        {
            Mock<IMessagingDomainService> messagingDomainService = new Mock<IMessagingDomainService>();
            MessagingRequestHandler handler = new MessagingRequestHandler(messagingDomainService.Object);

            ResendEmailRequest command = TestData.ResendEmailRequest;

            Should.NotThrow(async () => {
                                await handler.Handle(command, CancellationToken.None);
                            });
        }

        [Fact]
        public void MessagingRequestHandler_SendSMSRequest_IsHandled()
        {
            Mock<IMessagingDomainService> messagingDomainService = new Mock<IMessagingDomainService>();
            MessagingRequestHandler handler = new MessagingRequestHandler(messagingDomainService.Object);

            SendSMSRequest command = TestData.SendSMSRequest;

            Should.NotThrow(async () =>
                            {
                                await handler.Handle(command, CancellationToken.None);
                            });

        }
    }
}
