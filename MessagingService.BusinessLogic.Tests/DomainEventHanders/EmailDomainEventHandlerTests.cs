using System;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System.Threading;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using EventHandling;
    using Moq;
    using Shared.EventStore.EventStore;
    using System.Threading.Tasks;
    using Testing;
    using Xunit;

    public class EmailDomainEventHandlerTests
    {
        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseDelivered);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Failed_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseFailed);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseRejected);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Bounced_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseBounced);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Spam_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseSpam);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Unknown_EventIsHandled()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy.Setup(e => e.GetMessageStatus(It.IsAny<String>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(TestData.MessageStatusResponseUnknown);

            EmailDomainEventHandler emailDomainEventHandler = new EmailDomainEventHandler(aggregateRepository.Object,
                                                                                          emailServiceProxy.Object);

            await emailDomainEventHandler.Handle(TestData.EmailResponseReceivedFromProviderEvent, CancellationToken.None);
        }
    }
}
