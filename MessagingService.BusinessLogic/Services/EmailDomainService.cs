namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessageAggregate;
    using EmailServices;
    using Microsoft.Extensions.Logging;
    using Shared.EventStore.EventStore;
    using Shared.Logger;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MessagingService.BusinessLogic.Services.IEmailDomainService" />
    public class EmailDomainService : IEmailDomainService
    {
        #region Fields

        /// <summary>
        /// The email aggregate repository
        /// </summary>
        private readonly IAggregateRepository<EmailAggregate> EmailAggregateRepository;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainService" /> class.
        /// </summary>
        /// <param name="emailAggregateRepository">The email aggregate repository.</param>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        public EmailDomainService(IAggregateRepository<EmailAggregate> emailAggregateRepository,
                                  IEmailServiceProxy emailServiceProxy)
        {
            this.EmailAggregateRepository = emailAggregateRepository;
            this.EmailServiceProxy = emailServiceProxy;
            this.EmailAggregateRepository.TraceGenerated += this.EmailAggregateRepository_TraceGenerated;
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
            // Rehydrate Email Message aggregate
            EmailAggregate emailAggregate = await this.EmailAggregateRepository.GetLatestVersion(messageId, cancellationToken);

            // send message to provider (record event)
            emailAggregate.SendRequestToProvider(fromAddress, toAddresses, subject, body, isHtml);

            // Make call to Email provider here
            EmailServiceProxyResponse emailResponse =
                await this.EmailServiceProxy.SendEmail(messageId, fromAddress, toAddresses, subject, body, isHtml, cancellationToken);

            // response message from provider (record event)
            emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);

            // Save Changes to persistance
            await this.EmailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
        }

        /// <summary>
        /// Emails the aggregate repository trace generated.
        /// </summary>
        /// <param name="trace">The trace.</param>
        /// <param name="logLevel">The log level.</param>
        private void EmailAggregateRepository_TraceGenerated(String trace,
                                                             LogLevel logLevel)
        {
            switch(logLevel)
            {
                case LogLevel.Critical:
                    Logger.LogCritical(new Exception(trace));
                    break;
                case LogLevel.Debug:
                    Logger.LogDebug(trace);
                    break;
                case LogLevel.Error:
                    Logger.LogError(new Exception(trace));
                    break;
                case LogLevel.Information:
                    Logger.LogInformation(trace);
                    break;
                case LogLevel.Trace:
                    Logger.LogTrace(trace);
                    break;
                case LogLevel.Warning:
                    Logger.LogWarning(trace);
                    break;
            }
        }

        #endregion
    }
}