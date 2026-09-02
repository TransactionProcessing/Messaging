using System;
using System.Text;
using MediatR;

namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System.Threading;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using EventHandling;
    using Imposter.Abstractions;
    using System.Threading.Tasks;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Testing;
    using Xunit;

    public class EmailDomainEventHandlerTests {
        private IMediatorImposter Mediator;
        private IEmailServiceProxyImposter EmailServiceProxy;
        private EmailDomainEventHandler EmailDomainEventHandler;
        public EmailDomainEventHandlerTests() {
            this.Mediator = new IMediatorImposter();
            this.EmailServiceProxy = new IEmailServiceProxyImposter();
            this.EmailDomainEventHandler =
                new EmailDomainEventHandler(this.Mediator.Instance(), this.EmailServiceProxy.Instance());
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Delivered_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseDelivered);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Failed_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseFailed);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Rejected_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseRejected);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Bounced_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseBounced);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Spam_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseSpam);

            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }

        [Fact]
        public async Task EmailDomainEventHandler_Handle_ResponseReceivedFromProviderEvent_Unknown_EventIsHandled()
        {
            this.EmailServiceProxy.GetMessageStatus(Arg<String>.Any(), Arg<DateTime>.Any(), Arg<DateTime>.Any(), Arg<CancellationToken>.Any())
                             .ReturnsAsync(TestData.MessageStatusResponseUnknown);
            
            await this.EmailDomainEventHandler.Handle(TestData.ResponseReceivedFromEmailProviderEvent, CancellationToken.None);
        }
    }
}
