namespace MessagingService.SMSMessage.DomainEvents;

using System;
using Shared.DomainDrivenDesign.EventSourcing;

public record RequestSentToSMSProviderEvent(Guid MessageId,
                                            String Sender,
                                            String Destination,
                                            String Message) : DomainEvent(MessageId, Guid.NewGuid());

public record ResponseReceivedFromSMSProviderEvent(Guid MessageId,
                                                   String ProviderSMSReference) : DomainEvent(MessageId, Guid.NewGuid());

public record SMSMessageDeliveredEvent(Guid MessageId,
                                       String ProviderStatus,
                                       DateTime DeliveredDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record SMSMessageExpiredEvent(Guid MessageId,
                                     String ProviderStatus,
                                     DateTime ExpiredDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record SMSMessageRejectedEvent(Guid MessageId,
                                      String ProviderStatus,
                                      DateTime RejectedDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record SMSMessageUndeliveredEvent(Guid MessageId,
                                         String ProviderStatus,
                                         DateTime UndeliveredDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record RequestResentToSMSProviderEvent(Guid MessageId) : DomainEvent(MessageId, Guid.NewGuid());