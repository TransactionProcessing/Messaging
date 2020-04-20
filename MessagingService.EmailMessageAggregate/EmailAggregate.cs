namespace MessagingService.EmailMessageAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using EmailMessage.DomainEvents;
    using Microsoft.EntityFrameworkCore.Migrations.Operations;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.DomainDrivenDesign.EventStore;
    using Shared.General;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Shared.DomainDrivenDesign.EventStore.Aggregate" />
    public class EmailAggregate : Aggregate
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAggregate" /> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public EmailAggregate()
        {
            this.Recipients = new List<MessageRecipient>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAggregate" /> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        private EmailAggregate(Guid aggregateId)
        {
            Guard.ThrowIfInvalidGuid(aggregateId, "Aggregate Id cannot be an Empty Guid");

            this.AggregateId = aggregateId;
            this.Recipients = new List<MessageRecipient>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Messages the send to recipient failure.
        /// </summary>
        public void MessageSendToRecipientFailure()
        {
        }

        /// <summary>
        /// Messages the send to recipient successful.
        /// </summary>
        public void MessageSendToRecipientSuccessful()
        {
        }

        /// <summary>
        /// Receives the response from provider.
        /// </summary>
        /// <param name="providerRequestReference">The provider request reference.</param>
        /// <param name="providerEmailReference">The provider email reference.</param>
        public void ReceiveResponseFromProvider(String providerRequestReference,
                                                String providerEmailReference)
        {
            ResponseReceivedFromProviderEvent responseReceivedFromProviderEvent = ResponseReceivedFromProviderEvent.Create(this.AggregateId, providerRequestReference, providerEmailReference);

            this.ApplyAndPend(responseReceivedFromProviderEvent);
        }

        /// <summary>
        /// Sends the request to provider.
        /// </summary>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddresses">To addresses.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        public void SendRequestToProvider(String fromAddress,
                                          List<String> toAddresses,
                                          String subject,
                                          String body,
                                          Boolean isHtml)
        {
            RequestSentToProviderEvent requestSentToProviderEvent = RequestSentToProviderEvent.Create(this.AggregateId, fromAddress, toAddresses, subject, body, isHtml);

            this.ApplyAndPend(requestSentToProviderEvent);
        }

        public String ProviderRequestReference { get; private set; }
        public String ProviderEmailReference { get; private set; }
        public String FromAddress { get; private set; }
        public String Subject { get; private set; }
        public String Body { get; private set; }
        public Boolean IsHtml { get; private set; }

        private List<MessageRecipient> Recipients;


        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        protected override Object GetMetadata()
        {
            return null;
        }

        /// <summary>
        /// Plays the event.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        protected override void PlayEvent(DomainEvent domainEvent)
        {
            this.PlayEvent((dynamic)domainEvent);
        }

        private void PlayEvent(RequestSentToProviderEvent domainEvent)
        {
            this.Body = domainEvent.Body;
            this.Subject = domainEvent.Subject;
            this.IsHtml = domainEvent.IsHtml;
            this.FromAddress = domainEvent.FromAddress;

            foreach (String domainEventToAddress in domainEvent.ToAddresses)
            {
                MessageRecipient messageRecipient = new MessageRecipient();
                messageRecipient.Create(domainEventToAddress);
                this.Recipients.Add(messageRecipient);
            }
        }

        private void PlayEvent(ResponseReceivedFromProviderEvent domainEvent)
        {
            this.ProviderEmailReference = domainEvent.ProviderEmailReference;
            this.ProviderRequestReference = domainEvent.ProviderRequestReference;
        }

        #endregion
    }

    internal class MessageRecipient
    {
        internal String ToAddress { get; private set; }

        internal MessageRecipient()
        {
            
        }

        internal void Create(String toAddress)
        {
            this.ToAddress = toAddress;
        }
    }
}