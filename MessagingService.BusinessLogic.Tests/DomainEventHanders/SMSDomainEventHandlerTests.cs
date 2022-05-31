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

    public class SMSDomainEventHandlerTests
    {
        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> aggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();
            smsServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.SMSMessageStatusResponseDelivered);

            SMSDomainEventHandler smsDomainEventHandler = new SMSDomainEventHandler(aggregateRepository.Object,
                                                                                          smsServiceProxy.Object);

            await smsDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Expired_EventIsHandled()
        {
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> aggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();
            smsServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseExpired);

            SMSDomainEventHandler smsDomainEventHandler = new SMSDomainEventHandler(aggregateRepository.Object,
                                                                                    smsServiceProxy.Object);

            await smsDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> aggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();
            smsServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseRejected);

            SMSDomainEventHandler smsDomainEventHandler = new SMSDomainEventHandler(aggregateRepository.Object,
                                                                                    smsServiceProxy.Object);

            await smsDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Undelivered_EventIsHandled()
        {
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> aggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();
            smsServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(TestData.SMSMessageStatusResponseUndelivered);

            SMSDomainEventHandler smsDomainEventHandler = new SMSDomainEventHandler(aggregateRepository.Object,
                                                                                    smsServiceProxy.Object);

            await smsDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }
    }
}
