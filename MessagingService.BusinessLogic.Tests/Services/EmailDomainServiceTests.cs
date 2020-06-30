using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using Moq;
    using Shared.EventStore.EventStore;
    using Testing;
    using Xunit;

    public class EmailDomainServiceTests
    {
        [Fact]
        public async Task TransactionDomainService_ProcessLogonTransaction_TransactionIsProcessed()
        {
            Mock<IAggregateRepository<EmailAggregate>> aggregateRepository = new Mock<IAggregateRepository<EmailAggregate>>();
            aggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new Mock<IEmailServiceProxy>();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            //IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
            //ConfigurationReader.Initialise(configurationRoot);

            //Logger.Initialise(NullLogger.Instance);

            EmailDomainService emailDomainService =
                new EmailDomainService(aggregateRepository.Object, emailServiceProxy.Object);

            await emailDomainService.SendEmailMessage(TestData.ConnectionIdentifier,
                                                TestData.MessageId,
                                                TestData.FromAddress,
                                                TestData.ToAddresses,
                                                TestData.Subject,
                                                TestData.Body,
                                                TestData.IsHtmlTrue,
                                                CancellationToken.None);
        }

    }
}
