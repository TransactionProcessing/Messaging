namespace MessagingService.EmailMessage.DomainEvents;

using System;
using Shared.DomainDrivenDesign.EventSourcing;

public record RequestResentToEmailProviderEvent : DomainEvent
{
    public RequestResentToEmailProviderEvent(Guid aggregateId) : base(aggregateId, Guid.NewGuid())
    {
        this.MessageId = aggregateId;
    }

    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    /// <value>
    /// The message identifier.
    /// </value>
    public Guid MessageId { get; init; }
}