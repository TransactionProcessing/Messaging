using SimpleResults;

namespace MessagingService.BusinessLogic.EventHandling
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MessagingService.BusinessLogic.Requests;
    using Services.SMSServices;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.EventHandling;
    using SMSMessage.DomainEvents;
    using SMSMessageAggregate;
    using MessageStatus = Services.SMSServices.MessageStatus;

    public class SMSDomainEventHandler : IDomainEventHandler
    {
        #region Fields

        private readonly IMediator Mediator;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly ISMSServiceProxy SMSServiceProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainEventHandler" /> class.
        /// </summary>
        /// <param name="aggregateRepository">The aggregate repository.</param>
        /// <param name="smsServiceProxy">The SMS service proxy.</param>
        public SMSDomainEventHandler(IMediator mediator,
                                     ISMSServiceProxy smsServiceProxy) {
            this.Mediator = mediator;
            this.SMSServiceProxy = smsServiceProxy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Result> Handle(IDomainEvent domainEvent,
                                         CancellationToken cancellationToken)
        {
            return await this.HandleSpecificDomainEvent((dynamic)domainEvent, cancellationToken);
        }

        /// <summary>
        /// Handles the specific domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Result> HandleSpecificDomainEvent(ResponseReceivedFromSMSProviderEvent domainEvent,
                                                     CancellationToken cancellationToken)
        {
            // Get the message status from the provider
            MessageStatusResponse messageStatus = await this.SMSServiceProxy.GetMessageStatus(domainEvent.ProviderSMSReference,
                                                                                                cancellationToken);

            // Now update the aggregate
            SMSCommands.UpdateMessageStatusCommand command = new(domainEvent.MessageId, messageStatus.MessageStatus,
                messageStatus.ProviderStatusDescription, messageStatus.Timestamp);

            return await this.Mediator.Send(command, cancellationToken);
        }

        #endregion
    }
}