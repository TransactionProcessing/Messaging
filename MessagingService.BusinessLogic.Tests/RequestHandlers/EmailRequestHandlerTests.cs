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

    public class EmailRequestHandlerTests
    {
        [Fact]
        public void TransactionRequestHandler_ProcessLogonTransactionRequest_IsHandled()
        {
            Mock<IEmailDomainService> emailDomainService = new Mock<IEmailDomainService>();
            EmailRequestHandler handler = new EmailRequestHandler(emailDomainService.Object);

            SendEmailRequest command = TestData.SendEmailRequest;

            Should.NotThrow(async () =>
                            {
                                await handler.Handle(command, CancellationToken.None);
                            });

        }
    }
}
