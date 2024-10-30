using System;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System.Threading;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using EventHandling;
    using Moq;
    using System.Threading.Tasks;
    using BusinessLogic.Services.SMSServices;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using SMSMessageAggregate;
    using Testing;
    using Xunit;
    using MediatR;

    public class SMSDomainEventHandlerTests
    {
        private Mock<IMediator> Mediator;
        private Mock<ISMSServiceProxy> SMSServiceProxy;
        private SMSDomainEventHandler SMSDomainEventHandler;
        public SMSDomainEventHandlerTests()
        {
            this.Mediator = new Mock<IMediator>();
            this.SMSServiceProxy = new Mock<ISMSServiceProxy>();
            this.SMSDomainEventHandler =
                new SMSDomainEventHandler(this.Mediator.Object, this.SMSServiceProxy.Object);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            this.SMSServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.SMSMessageStatusResponseDelivered);
            
            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Expired_EventIsHandled()
        {
            this.SMSServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseExpired);
            
            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            this.SMSServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseRejected);

            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Undelivered_EventIsHandled()
        {
            this.SMSServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseUndelivered);

            await this.SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }
    }
}
