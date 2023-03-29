using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using BusinessLogic.Services.EmailServices;
    using BusinessLogic.Services.SMSServices;
    using EmailMessageAggregate;
    using Models;
    using Moq;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using SMSMessageAggregate;
    using Testing;
    using Xunit;

    public class MessagingDomainServiceTests
    {
        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new Mock<IAggregateRepository<EmailAggregate, DomainEvent>>();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendEmailMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          TestData.FromAddress,
                                                          TestData.ToAddresses,
                                                          TestData.Subject,
                                                          TestData.Body,
                                                          TestData.IsHtmlTrue,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new Mock<IAggregateRepository<EmailAggregate, DomainEvent>>();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate);
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendSMSMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new Mock<IAggregateRepository<EmailAggregate, DomainEvent>>();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new Mock<IAggregateRepository<SMSAggregate, DomainEvent>>();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptySMSAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            Mock<ISMSServiceProxy> smsServiceProxy = new Mock<ISMSServiceProxy>();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendSMSMessage(TestData.ConnectionIdentifier,
                                                        TestData.MessageId,
                                                        TestData.Sender,
                                                        TestData.Destination,
                                                        TestData.Message,
                                                        CancellationToken.None);
        }

    }
}
