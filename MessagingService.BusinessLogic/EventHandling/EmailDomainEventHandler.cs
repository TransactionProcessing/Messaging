using MediatR;
using MessagingService.BusinessLogic.Requests;
using SimpleResults;

namespace MessagingService.BusinessLogic.EventHandling
{
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessage.DomainEvents;
    using EmailMessageAggregate;
    using Services.EmailServices;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.EventStore.EventHandling;
    using MessageStatus = Services.EmailServices.MessageStatus;

    public class EmailDomainEventHandler : IDomainEventHandler
    {
        #region Fields

        private readonly IMediator Mediator;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        #endregion

        #region Constructors

        public EmailDomainEventHandler(IMediator mediator,
                                       IEmailServiceProxy emailServiceProxy)
        {
            this.Mediator = mediator;
            this.EmailServiceProxy = emailServiceProxy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Result> Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return await this.HandleSpecificDomainEvent((dynamic)domainEvent, cancellationToken);
        }

        /// <summary>
        /// Handles the specific domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task<Result> HandleSpecificDomainEvent(ResponseReceivedFromEmailProviderEvent domainEvent,
                                                     CancellationToken cancellationToken)
        {
            // Get the message status from the provider
            MessageStatusResponse messageStatus = await this.EmailServiceProxy.GetMessageStatus(domainEvent.ProviderEmailReference,
                                                                                                domainEvent.EventTimestamp.DateTime,
                                                                                                domainEvent.EventTimestamp.DateTime,
                                                                                                cancellationToken);

            // Now update the aggregate
            EmailCommands.UpdateMessageStatusCommand command = new(domainEvent.MessageId, messageStatus.MessageStatus,
                messageStatus.ProviderStatusDescription, messageStatus.Timestamp);

            return await this.Mediator.Send(command, cancellationToken);
        }

        #endregion
    }
}