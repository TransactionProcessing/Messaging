namespace MessagingService.EmailMessage.DomainEvents;

using System;
using System.Collections.Generic;
using Shared.DomainDrivenDesign.EventSourcing;

public record ResponseReceivedFromEmailProviderEvent(Guid MessageId,
                                                     String ProviderRequestReference,
                                                     String ProviderEmailReference) : DomainEvent(MessageId, Guid.NewGuid());

public record BadResponseReceivedFromEmailProviderEvent(Guid MessageId,
                                                        String ErrorCode,
                                                        String Error) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailMessageBouncedEvent(Guid MessageId,
                                       String ProviderStatus,
                                       DateTime BouncedDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailMessageDeliveredEvent(Guid MessageId,
                                         String ProviderStatus,
                                         DateTime DeliveredDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailMessageFailedEvent(Guid MessageId,
                                      String ProviderStatus,
                                      DateTime FailedDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailMessageMarkedAsSpamEvent(Guid MessageId,
                                            String ProviderStatus,
                                            DateTime SpamDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailMessageRejectedEvent(Guid MessageId,
                                        String ProviderStatus,
                                        DateTime RejectedDateTime) : DomainEvent(MessageId, Guid.NewGuid());

public record RequestResentToEmailProviderEvent(Guid MessageId) : DomainEvent(MessageId, Guid.NewGuid());

public record EmailAttachmentRequestSentToProviderEvent(Guid MessageId, String Filename, String FileData, Int32 FileType) : DomainEvent(MessageId, Guid.NewGuid());

public record RequestSentToEmailProviderEvent(Guid MessageId,
                                              String FromAddress,
                                              List<String> ToAddresses,
                                              String Subject,
                                              String Body,
                                              Boolean IsHtml) : DomainEvent(MessageId, Guid.NewGuid());