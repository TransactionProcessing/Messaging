namespace MessagingService.BusinessLogic.EventHandling
{
    using System.Threading;
    using System.Threading.Tasks;
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

        /// <summary>
        /// The aggregate repository
        /// </summary>
        private readonly IAggregateRepository<SMSAggregate, DomainEvent> AggregateRepository;

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
        public SMSDomainEventHandler(IAggregateRepository<SMSAggregate, DomainEvent> aggregateRepository,
                                     ISMSServiceProxy smsServiceProxy)
        {
            this.AggregateRepository = aggregateRepository;
            this.SMSServiceProxy = smsServiceProxy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task Handle(IDomainEvent domainEvent,
                                 CancellationToken cancellationToken)
        {
            await this.HandleSpecificDomainEvent((dynamic)domainEvent, cancellationToken);
        }

        /// <summary>
        /// Handles the specific domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task HandleSpecificDomainEvent(ResponseReceivedFromSMSProviderEvent domainEvent,
                                                     CancellationToken cancellationToken)
        {
            SMSAggregate smsAggregate = await this.AggregateRepository.GetLatestVersion(domainEvent.MessageId, cancellationToken);

            // Update the aggregate with the status request information

            // Get the message status from the provider
            MessageStatusResponse messageStatus = await this.SMSServiceProxy.GetMessageStatus(domainEvent.ProviderSMSReference,
                                                                                                cancellationToken);

            // Update the aggregate with the response
            switch (messageStatus.MessageStatus)
            {
                case MessageStatus.Expired:
                    smsAggregate.MarkMessageAsExpired(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Rejected:
                    smsAggregate.MarkMessageAsRejected(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Undeliverable:
                    smsAggregate.MarkMessageAsUndeliverable(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Delivered:
                    smsAggregate.MarkMessageAsDelivered(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
            }

            // Save the changes
            await this.AggregateRepository.SaveChanges(smsAggregate, cancellationToken);
        }

        #endregion
    }
}