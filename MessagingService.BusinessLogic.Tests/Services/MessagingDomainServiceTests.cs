using System;
using System.Collections.Generic;
using System.Text;
using MessagingService.BusinessLogic.Requests;

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
    using Shouldly;
    using SimpleResults;
    using SMSMessageAggregate;
    using Testing;
    using Xunit;

    public class MessagingDomainServiceTests
    {
        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            emailAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<EmailAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success);
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            var result  = await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_SecondSend_MessageNotSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate());
            emailAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<EmailAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success);
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService = new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            var result = await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                TestData.EmailAttachmentModels,
                CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(TestData.MessageId);
        }

        [Fact] public async Task MessagingDomainService_SendEmailMessage_SaveFailed_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            emailAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<EmailAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure);
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_EmailSentFailed_APICallFailed_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedAPICallEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_EmailSentFailed_APIResponseError_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_APICallFailed_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedAPICallEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                            TestData.MessageId,
                                                            CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_APIResponseError_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            emailServiceProxy
                .Setup(e => e.SendEmail(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<List<String>>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<Boolean>(),
                                        It.IsAny<List<EmailAttachment>>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedEmailServiceProxyResponse);
            Mock<ISMSServiceProxy> smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                            TestData.MessageId,
                                                            CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendSMSMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptySMSAggregate());
            smsAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<SMSAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<String>(),
                                        It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            var result = await messagingDomainService.SendSMSMessage(TestData.ConnectionIdentifier,
                                                        TestData.MessageId,
                                                        TestData.Sender,
                                                        TestData.Destination,
                                                        TestData.Message,
                                                        CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(TestData.MessageId);

        }

        [Fact]
        public async Task MessagingDomainService_SendSMSMessage_SecondTime_MessageNotSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate());
            smsAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<SMSAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            var result = await messagingDomainService.SendSMSMessage(TestData.ConnectionIdentifier,
                TestData.MessageId,
                TestData.Sender,
                TestData.Destination,
                TestData.Message,
                CancellationToken.None);
            result.IsSuccess.ShouldBeTrue(); 
            result.Data.ShouldBe(TestData.MessageId);

        }

        [Fact]
        public async Task MessagingDomainService_SendSMSMessage_SaveFailed_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetEmptySMSAggregate());
            smsAggregateRepository.Setup(a => a.SaveChanges(It.IsAny<SMSAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure);
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.SendSMSMessage(TestData.ConnectionIdentifier,
                TestData.MessageId,
                TestData.Sender,
                TestData.Destination,
                TestData.Message,
                CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ReSendSMSMessage_MessageSent()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendSMSMessage(TestData.ConnectionIdentifier,
                                                        TestData.MessageId,
                                                        CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ReSendSMSMessage_APICallFailed_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedAPICallSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendSMSMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ReSendSMSMessage_APIResponseError_MessageFailed()
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            smsServiceProxy
                .Setup(e => e.SendSMS(It.IsAny<Guid>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<String>(),
                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.FailedSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService = new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            await messagingDomainService.ResendSMSMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Theory]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Delivered)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.InProgress)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Expired)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Rejected)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Sent)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Undeliverable)]
        [InlineData(BusinessLogic.Services.SMSServices.MessageStatus.Incoming)]
        public async Task MessagingDomainService_UpdateSMSMessageStatus_MessageUpdated(BusinessLogic.Services.SMSServices.MessageStatus status)
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            smsAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentSMSAggregate());
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            SMSCommands.UpdateMessageStatusCommand command = new(TestData.MessageId, status, TestData.ProviderStatusDescription, TestData.BouncedDateTime);
            Should.NotThrow(async () => await messagingDomainService.UpdateMessageStatus(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(BusinessLogic.Services.EmailServices.MessageStatus.Delivered)]
        [InlineData(BusinessLogic.Services.EmailServices.MessageStatus.Rejected)]
        [InlineData(BusinessLogic.Services.EmailServices.MessageStatus.Bounced)]
        [InlineData(BusinessLogic.Services.EmailServices.MessageStatus.Failed)]
        [InlineData(BusinessLogic.Services.EmailServices.MessageStatus.Spam)]

        public async Task MessagingDomainService_UpdateEmailMessageStatus_MessageUpdated(BusinessLogic.Services.EmailServices.MessageStatus status)
        {
            Mock<IAggregateRepository<EmailAggregate, DomainEvent>> emailAggregateRepository = new();
            emailAggregateRepository.Setup(a => a.GetLatestVersion(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.GetSentEmailAggregate());
            Mock<IAggregateRepository<SMSAggregate, DomainEvent>> smsAggregateRepository = new();
            Mock<IEmailServiceProxy> emailServiceProxy = new();
            Mock<ISMSServiceProxy> smsServiceProxy = new();
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Object, smsAggregateRepository.Object, emailServiceProxy.Object, smsServiceProxy.Object);

            EmailCommands.UpdateMessageStatusCommand command = new(TestData.MessageId, status, TestData.ProviderStatusDescription, TestData.BouncedDateTime);
            Should.NotThrow(async () => {
                await messagingDomainService.UpdateMessageStatus(command, CancellationToken.None);
            });
        }
    }

    public class DomainServiceHelperTests
    {
        [Fact]
        public void DomainServiceHelper_HandleGetAggregateResult_SuccessfulGet_ResultHandled()
        {
            Guid aggregateId = Guid.Parse("0639682D-1D28-4AD8-B29D-4B76619083F1");
            Result<TestAggregate> result = Result.Success(new TestAggregate
            {
                AggregateId = aggregateId
            });

            var handleResult = DomainServiceHelper.HandleGetAggregateResult(result, aggregateId, true);
            handleResult.IsSuccess.ShouldBeTrue();
            handleResult.Data.ShouldBeOfType(typeof(TestAggregate));
            handleResult.Data.AggregateId.ShouldBe(aggregateId);
        }

        [Fact]
        public void DomainServiceHelper_HandleGetAggregateResult_FailedGet_ResultHandled()
        {
            Guid aggregateId = Guid.Parse("0639682D-1D28-4AD8-B29D-4B76619083F1");
            Result<TestAggregate> result = Result.Failure("Failed Get");

            var handleResult = DomainServiceHelper.HandleGetAggregateResult(result, aggregateId, true);
            handleResult.IsFailed.ShouldBeTrue();
            handleResult.Message.ShouldBe("Failed Get");
        }

        [Fact]
        public void DomainServiceHelper_HandleGetAggregateResult_FailedGet_NotFoundButIsError_ResultHandled()
        {
            Guid aggregateId = Guid.Parse("0639682D-1D28-4AD8-B29D-4B76619083F1");
            Result<TestAggregate> result = Result.NotFound("Failed Get");

            var handleResult = DomainServiceHelper.HandleGetAggregateResult(result, aggregateId, true);
            handleResult.IsFailed.ShouldBeTrue();
            handleResult.Message.ShouldBe("Failed Get");
        }

        [Fact]
        public void DomainServiceHelper_HandleGetAggregateResult_FailedGet_NotFoundButIsNotError_ResultHandled()
        {
            Guid aggregateId = Guid.Parse("0639682D-1D28-4AD8-B29D-4B76619083F1");
            Result<TestAggregate> result = Result.NotFound("Failed Get");

            var handleResult = DomainServiceHelper.HandleGetAggregateResult(result, aggregateId, false);
            handleResult.IsSuccess.ShouldBeTrue();
            handleResult.Data.ShouldBeOfType(typeof(TestAggregate));
            handleResult.Data.AggregateId.ShouldBe(aggregateId);
        }
    }

    public record TestAggregate : Aggregate
    {
        public override void PlayEvent(IDomainEvent domainEvent)
        {

        }

        protected override Object GetMetadata()
        {
            return new Object();
        }
    }

}
