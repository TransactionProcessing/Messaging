using System;
using System.Text;
using MediatR;

namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System.Threading;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using EventHandling;
    using Moq;
    using System.Threading.Tasks;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Testing;
    using Xunit;

    public class EmailDomainEventHandlerTests {
        private Mock<IMediator> Mediator;
        private Mock<IEmailServiceProxy> EmailServiceProxy;
        private EmailDomainEventHandler EmailDomainEventHandler;
        public EmailDomainEventHandlerTests() {
            this.Mediator = new Mock<IMediator>();
            this.EmailServiceProxy = new Mock<IEmailServiceProxy>();
            this.EmailDomainEventHandler =
                new EmailDomainEventHandler(this.Mediator.Object, this.EmailServiceProxy.Object);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseDelivered);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Failed_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseFailed);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseRejected);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Bounced_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseBounced);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Spam_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseSpam);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Unknown_EventIsHandled()
        {
            this.EmailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseUnknown);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }
    }
}
