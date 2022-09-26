namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessage.DomainEvents;
    using EmailMessageAggregate;
    using EmailServices;
    using Microsoft.Extensions.Logging;
    using Requests;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using SMSMessageAggregate;
    using SMSServices;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IMessagingDomainService" />
    public class MessagingDomainService : IMessagingDomainService
    {
        #region Fields

        /// <summary>
        /// The email aggregate repository
        /// </summary>
        private readonly IAggregateRepository<EmailAggregate, DomainEvent> EmailAggregateRepository;

        /// <summary>
        /// The SMS aggregate repository
        /// </summary>
        private readonly IAggregateRepository<SMSAggregate, DomainEvent> SmsAggregateRepository;

        /// <summary>
        /// The email service proxy
        /// </summary>
        private readonly IEmailServiceProxy EmailServiceProxy;

        /// <summary>
        /// The SMS service proxy
        /// </summary>
        private readonly ISMSServiceProxy SmsServiceProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingDomainService" /> class.
        /// </summary>
        /// <param name="emailAggregateRepository">The email aggregate repository.</param>
        /// <param name="smsAggregateRepository">The SMS aggregate repository.</param>
        /// <param name="emailServiceProxy">The email service proxy.</param>
        /// <param name="smsServiceProxy">The SMS service proxy.</param>
        public MessagingDomainService(IAggregateRepository<EmailAggregate, DomainEvent> emailAggregateRepository,
                                      IAggregateRepository<SMSAggregate, DomainEvent> smsAggregateRepository,
                                      IEmailServiceProxy emailServiceProxy,
                                      ISMSServiceProxy smsServiceProxy)
        {
            this.EmailAggregateRepository = emailAggregateRepository;
            this.SmsAggregateRepository = smsAggregateRepository;

            this.EmailServiceProxy = emailServiceProxy;
            this.SmsServiceProxy = smsServiceProxy;
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
                                           List<EmailAttachment> attachments,
                                           CancellationToken cancellationToken)
        {
            // Rehydrate Email Message aggregate
            EmailAggregate emailAggregate = await this.EmailAggregateRepository.GetLatestVersion(messageId, cancellationToken);

            // send message to provider (record event)
            emailAggregate.SendRequestToProvider(fromAddress, toAddresses, subject, body, isHtml);

            // Make call to Email provider here
            EmailServiceProxyResponse emailResponse =
                await this.EmailServiceProxy.SendEmail(messageId, fromAddress, toAddresses, subject, body, isHtml, attachments, cancellationToken);

            // response message from provider (record event)
            emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);

            // Save Changes to persistance
            await this.EmailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
        }

        /// <summary>
        /// Sends the SMS message.
        /// </summary>
        /// <param name="connectionIdentifier">The connection identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SendSMSMessage(Guid connectionIdentifier,
                                         Guid messageId,
                                         String sender,
                                         String destination,
                                         String message,
                                         CancellationToken cancellationToken)
        {
            // Rehydrate SMS Message aggregate
            SMSAggregate smsAggregate = await this.SmsAggregateRepository.GetLatestVersion(messageId, cancellationToken);

            // send message to provider (record event)
            smsAggregate.SendRequestToProvider(sender, destination,message);

            // Make call to SMS provider here
            SMSServiceProxyResponse smsResponse =
                await this.SmsServiceProxy.SendSMS(messageId, sender, destination, message, cancellationToken);

            // response message from provider (record event)
            smsAggregate.ReceiveResponseFromProvider(smsResponse.SMSIdentifier);

            // Save Changes to persistance
            await this.SmsAggregateRepository.SaveChanges(smsAggregate, cancellationToken);
        }

        public async  Task ResendEmailMessage(Guid connectionIdentifier,
                                     Guid messageId,
                                     CancellationToken cancellationToken) {
            // Rehydrate Email Message aggregate
            EmailAggregate emailAggregate = await this.EmailAggregateRepository.GetLatestVersion(messageId, cancellationToken);

            // re-send message to provider (record event)
            emailAggregate.ResendRequestToProvider();

            // Make call to Email provider here
            EmailServiceProxyResponse emailResponse =
                await this.EmailServiceProxy.SendEmail(messageId, emailAggregate.FromAddress, 
                                                       emailAggregate.GetToAddresses(),
                                                       emailAggregate.Subject,
                                                       emailAggregate.Body, 
                                                       emailAggregate.IsHtml, null,
                                                       cancellationToken);

            // response message from provider (record event)
            emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);

            // Save Changes to persistance
            await this.EmailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
        }
    }

        #endregion
}