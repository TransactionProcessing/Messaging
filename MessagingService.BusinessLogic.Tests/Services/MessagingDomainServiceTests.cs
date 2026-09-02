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
    using Imposter.Abstractions;
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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            emailAggregateRepository.SaveChanges(Arg<EmailAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            var result  = await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(TestData.MessageId);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_SecondSend_MessageNotSent()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentEmailAggregate());
            emailAggregateRepository.SaveChanges(Arg<EmailAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService = new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            var result = await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                TestData.EmailAttachmentModels,
                CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe(TestData.MessageId);
        }

        [Fact] public async Task MessagingDomainService_SendEmailMessage_SaveFailed_MessageSent()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            emailAggregateRepository.SaveChanges(Arg<EmailAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_EmailSentFailed_APICallFailed_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedAPICallEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendEmailMessage_EmailSentFailed_APIResponseError_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptyEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.SendEmailMessage(TestData.SendEmailCommand,
                                                          TestData.EmailAttachmentModels,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_MessageSent()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_APICallFailed_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedAPICallEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                            TestData.MessageId,
                                                            CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ResendEmailMessage_APIResponseError_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            emailServiceProxy
                .SendEmail(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<List<String>>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<Boolean>.Any(),
                                        Arg<List<EmailAttachment>>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedEmailServiceProxyResponse);
            ISMSServiceProxyImposter smsServiceProxy = new();

            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.ResendEmailMessage(TestData.ConnectionIdentifier,
                                                            TestData.MessageId,
                                                            CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_SendSMSMessage_MessageSent()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptySMSAggregate());
            smsAggregateRepository.SaveChanges(Arg<SMSAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<String>.Any(),
                                        Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentSMSAggregate());
            smsAggregateRepository.SaveChanges(Arg<SMSAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                    Arg<String>.Any(),
                    Arg<String>.Any(),
                    Arg<String>.Any(),
                    Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetEmptySMSAggregate());
            smsAggregateRepository.SaveChanges(Arg<SMSAggregate>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                    Arg<String>.Any(),
                    Arg<String>.Any(),
                    Arg<String>.Any(),
                    Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentSMSAggregate());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.SuccessfulSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.ResendSMSMessage(TestData.ConnectionIdentifier,
                                                        TestData.MessageId,
                                                        CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ReSendSMSMessage_APICallFailed_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentSMSAggregate());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedAPICallSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService =
                new MessagingDomainService(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

            await messagingDomainService.ResendSMSMessage(TestData.ConnectionIdentifier,
                                                          TestData.MessageId,
                                                          CancellationToken.None);
        }

        [Fact]
        public async Task MessagingDomainService_ReSendSMSMessage_APIResponseError_MessageFailed()
        {
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentSMSAggregate());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            smsServiceProxy
                .SendSMS(Arg<Guid>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<String>.Any(),
                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.FailedSMSServiceProxyResponse);
            MessagingDomainService messagingDomainService = new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            smsAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentSMSAggregate());
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
            IAggregateRepositoryImposter<EmailAggregate, DomainEvent> emailAggregateRepository = new();
            emailAggregateRepository.GetLatestVersion(Arg<Guid>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(TestData.GetSentEmailAggregate());
            IAggregateRepositoryImposter<SMSAggregate, DomainEvent> smsAggregateRepository = new();
            IEmailServiceProxyImposter emailServiceProxy = new();
            ISMSServiceProxyImposter smsServiceProxy = new();
            MessagingDomainService messagingDomainService =
                new(emailAggregateRepository.Instance(), smsAggregateRepository.Instance(), emailServiceProxy.Instance(), smsServiceProxy.Instance());

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
