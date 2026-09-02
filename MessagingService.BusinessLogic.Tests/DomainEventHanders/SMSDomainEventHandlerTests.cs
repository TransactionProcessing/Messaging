using System;
using System.Text;

namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System.Threading;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using EventHandling;
    using Imposter.Abstractions;
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
        private IMediatorImposter Mediator;
        private ISMSServiceProxyImposter SMSServiceProxy;
        private SMSDomainEventHandler SMSDomainEventHandler;
        public SMSDomainEventHandlerTests()
        {
            this.Mediator = new IMediatorImposter();
            this.SMSServiceProxy = new ISMSServiceProxyImposter();
            this.SMSDomainEventHandler =
                new SMSDomainEventHandler(this.Mediator.Instance(), this.SMSServiceProxy.Instance());
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            this.SMSServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.SMSMessageStatusResponseDelivered);
            
            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Expired_EventIsHandled()
        {
            this.SMSServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<CancellationToken>.Any())
                           .ReturnsAsync(TestData.SMSMessageStatusResponseExpired);
            
            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            this.SMSServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<CancellationToken>.Any())
                           .ReturnsAsync(TestData.SMSMessageStatusResponseRejected);

            await SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SMSDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Undelivered_EventIsHandled()
        {
            this.SMSServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<CancellationToken>.Any())
                           .ReturnsAsync(TestData.SMSMessageStatusResponseUndelivered);

            await this.SMSDomainEventHandler.Handle(TestData.ResponseReceivedFromSMSProviderEvent, CancellationToken.None);
        }
    }
}
