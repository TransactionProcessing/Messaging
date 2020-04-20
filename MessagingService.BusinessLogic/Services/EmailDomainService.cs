namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessageAggregate;
    using EmailServices;
    using Shared.DomainDrivenDesign.EventStore;
    using Shared.EventStore.EventStore;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.IEmailDomainService" />
    public class EmailDomainService : IEmailDomainService
    {
        #region Fields

        /// <summary>
        /// The aggregate repository manager
        /// </summary>
        private readonly IAggregateRepositoryManager AggregateRepositoryManager;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainService"/> class.
        /// </summary>
        /// <param name="aggregateRepositoryManager">The aggregate repository manager.</param>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        public EmailDomainService(IAggregateRepositoryManager aggregateRepositoryManager,
                                  IEmailServiceProxy emailServiceProxy)
        {
            this.AggregateRepositoryManager = aggregateRepositoryManager;
            this.EmailServiceProxy = emailServiceProxy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the email message.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SendEmailMessage(Guid connectionIdentifier,
                                           Guid messageId,
                                           String fromAddress,
                                           List<String> toAddresses,
                                           String subject,
                                           String body,
                                           Boolean isHtml,
                                           CancellationToken cancellationToken)
        {
            IAggregateRepository<EmailAggregate> emailAggregateRepository = this.AggregateRepositoryManager.GetAggregateRepository<EmailAggregate>(connectionIdentifier);

            // Rehydrate Email Message aggregate
            EmailAggregate emailAggregate = await emailAggregateRepository.GetLatestVersion(messageId, cancellationToken);

            // send message to provider (record event)
            emailAggregate.SendRequestToProvider(fromAddress, toAddresses, subject, body, isHtml);

            // Make call to Email provider here
            EmailServiceProxyResponse emailResponse =
                await this.EmailServiceProxy.SendEmail(messageId, fromAddress, toAddresses, subject, body, isHtml, cancellationToken);

            // response message from provider (record event)
            emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);

            // Save Changes to persistance
            await emailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
        }

        #endregion
    }
}