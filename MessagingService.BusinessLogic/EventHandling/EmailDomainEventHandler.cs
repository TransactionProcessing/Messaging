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

        /// <summary>
        /// The aggregate repository
        /// </summary>
        private readonly IAggregateRepository<EmailAggregate, DomainEventRecord.DomainEvent> AggregateRepository;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainEventHandler"/> class.
        /// </summary>
        /// <param name="aggregateRepository">The aggregate repository.</param>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        public EmailDomainEventHandler()
                            //IAggregateRepository<EmailAggregate, DomainEventRecord.DomainEvent> aggregateRepository,
                            //           IEmailServiceProxy emailServiceProxy)
        {
            //this.AggregateRepository = aggregateRepository;
            //this.EmailServiceProxy = emailServiceProxy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await this.HandleSpecificDomainEvent((dynamic)domainEvent, cancellationToken);
        }

        /// <summary>
        /// Handles the specific domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task HandleSpecificDomainEvent(ResponseReceivedFromEmailProviderEvent domainEvent,
                                                     CancellationToken cancellationToken)
        {
            EmailAggregate emailAggregate = await this.AggregateRepository.GetLatestVersion(domainEvent.MessageId, cancellationToken);

            // Update the aggregate with the status request information

            // Get the message status from the provider
            MessageStatusResponse messageStatus = await this.EmailServiceProxy.GetMessageStatus(domainEvent.ProviderEmailReference,
                                                                                                domainEvent.EventTimestamp.DateTime,
                                                                                                domainEvent.EventTimestamp.DateTime,
                                                                                                cancellationToken);

            // Update the aggregate with the response
            switch (messageStatus.MessageStatus)
            {
                case MessageStatus.Failed:
                    emailAggregate.MarkMessageAsFailed(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Rejected:
                    emailAggregate.MarkMessageAsRejected(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Bounced:
                    emailAggregate.MarkMessageAsBounced(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Spam:
                    emailAggregate.MarkMessageAsSpam(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Delivered:
                    emailAggregate.MarkMessageAsDelivered(messageStatus.ProviderStatusDescription, messageStatus.Timestamp);
                    break;
                case MessageStatus.Unknown:
                    break;
            }

            // Save the changes
            await this.AggregateRepository.SaveChanges(emailAggregate, cancellationToken);
        }

        #endregion
    }
}