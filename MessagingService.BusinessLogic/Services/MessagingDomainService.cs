using MessagingService.BusinessLogic.Requests;
using TransactionProcessing.UnionResults;

namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessageAggregate;
    using EmailServices;
    using Models;
    using TransactionProcessing.Shared.DomainDrivenDesign.EventSourcing;
    using TransactionProcessing.Shared.EventStore.Aggregate;
    using TransactionProcessing.Shared.Exceptions;
    using SMSMessageAggregate;
    using SMSServices;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IMessagingDomainService" />
    public class MessagingDomainService : IMessagingDomainService
    {
        #region Fields
        
        private readonly IAggregateRepository<EmailAggregate, DomainEvent> EmailAggregateRepository;
        
        private readonly IAggregateRepository<SMSAggregate, DomainEvent> SmsAggregateRepository;
        
        private readonly IEmailServiceProxy EmailServiceProxy;
        
        private readonly ISMSServiceProxy SmsServiceProxy;

        #endregion

        #region Constructors
        
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

        private async Task<Result> ApplyEmailUpdates(Func<EmailAggregate, Task<Result>> action,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken,
                                                     Boolean isNotFoundError = true) {
            try {

                Result<EmailAggregate> getEmailResult = await this.EmailAggregateRepository.GetLatestVersion(messageId, cancellationToken);
                Result<EmailAggregate> emailAggregateResult =
                    DomainServiceHelper.HandleGetAggregateResult(getEmailResult, messageId, isNotFoundError);

                if (emailAggregateResult.Value is not Success<EmailAggregate> success)
                    return emailAggregateResult.Bind(static _ => Results.Ok());

                EmailAggregate emailAggregate = success.Value;
                Result result = await action(emailAggregate);
                if (!result.IsSuccess())
                    return result;

                return await this.EmailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
            }
            catch (Exception ex)
            {
                return new ErrorList(ex.GetExceptionMessages().Select(message => new Error(ErrorType.Failure, message)));
            }
        }

        private async Task<Result> ApplySMSUpdates(Func<SMSAggregate, Task<Result>> action,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken,
                                                     Boolean isNotFoundError = true)
        {
            try
            {

                Result<SMSAggregate> getSMSResult = await this.SmsAggregateRepository.GetLatestVersion(messageId, cancellationToken);
                Result<SMSAggregate> smsAggregateResult =
                    DomainServiceHelper.HandleGetAggregateResult(getSMSResult, messageId, isNotFoundError);

                if (smsAggregateResult.Value is not Success<SMSAggregate> success)
                    return smsAggregateResult.Bind(static _ => Results.Ok());

                SMSAggregate smsAggregate = success.Value;
                Result result = await action(smsAggregate);
                if (!result.IsSuccess())
                    return result;

                return await this.SmsAggregateRepository.SaveChanges(smsAggregate, cancellationToken);
            }
            catch (Exception ex)
            {
                return new ErrorList(ex.GetExceptionMessages().Select(message => new Error(ErrorType.Failure, message)));
            }
        }

        public async Task<Result<Guid>> SendEmailMessage(EmailCommands.SendEmailCommand command,
                                                         List<EmailAttachment> attachments,
                                                         CancellationToken cancellationToken) {
            Result result = await ApplyEmailUpdates(async (EmailAggregate emailAggregate) =>
            {
                // Check if this message has been sent before
                if (emailAggregate.GetMessageStatus() != EmailMessageAggregate.MessageStatus.NotSet) {
                    return Results.Ok();
                }

                // send message to provider (record event)
                Result stateResult = emailAggregate.SendRequestToProvider(command.FromAddress, command.ToAddresses, command.Subject, command.Body, command.IsHtml, attachments);
                if (!stateResult.IsSuccess())
                    return stateResult;

                // Make call to Email provider here
                EmailServiceProxyResponse emailResponse = await this.EmailServiceProxy.SendEmail(command.MessageId, command.FromAddress,
                    command.ToAddresses,command.Subject, command.Body, command.IsHtml, attachments, cancellationToken);

                if (emailResponse.ApiCallSuccessful) {
                    // response message from provider (record event)
                    stateResult = emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);
                }
                else {
                    stateResult = emailAggregate.ReceiveBadResponseFromProvider(emailResponse.Error, emailResponse.ErrorCode);
                }

                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, command.MessageId, cancellationToken, false);

            return result.Bind(() => Results.Ok(command.MessageId));
        }

        public async Task<Result<Guid>> SendSMSMessage(Guid connectionIdentifier,
                                                       Guid messageId,
                                                       String sender,
                                                       String destination,
                                                       String message,
                                                       CancellationToken cancellationToken)
        {
            Result result = await ApplySMSUpdates(async (SMSAggregate smsAggregate) => {
                // Check if this message has been sent before
                if (smsAggregate.GetMessageStatus() != SMSMessageAggregate.MessageStatus.NotSet) {
                    return Results.Ok();
                }

                // send message to provider (record event)
                Result stateResult = smsAggregate.SendRequestToProvider(sender, destination, message);
                if (!stateResult.IsSuccess())
                    return stateResult;

                // Make call to SMS provider here
                SMSServiceProxyResponse smsResponse =
                    await this.SmsServiceProxy.SendSMS(messageId, sender, destination, message, cancellationToken);

                // response message from provider (record event)
                stateResult= smsAggregate.ReceiveResponseFromProvider(smsResponse.SMSIdentifier);
                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, messageId, cancellationToken, false);

            return result.Bind(() => Results.Ok(messageId));

        }

        public async Task<Result> ResendEmailMessage(Guid connectionIdentifier,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken) {

            Result result = await ApplyEmailUpdates(async (EmailAggregate emailAggregate) => {
                // re-send message to provider (record event)
                Result stateResult = emailAggregate.ResendRequestToProvider();
                if (!stateResult.IsSuccess())
                    return stateResult;

                // Make call to Email provider here
                EmailServiceProxyResponse emailResponse =
                    await this.EmailServiceProxy.SendEmail(messageId, emailAggregate.FromAddress,
                                                           emailAggregate.GetToAddresses(),
                                                           emailAggregate.Subject,
                                                           emailAggregate.Body,
                                                           emailAggregate.IsHtml, null,
                                                           cancellationToken);

                if (emailResponse.ApiCallSuccessful) {
                    // response message from provider (record event)
                    stateResult = emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier, emailResponse.EmailIdentifier);
                }
                else {
                    stateResult = emailAggregate.ReceiveBadResponseFromProvider(emailResponse.Error, emailResponse.ErrorCode);
                }
                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, messageId, cancellationToken);
            return result;
        }

        public async Task<Result> ResendSMSMessage(Guid connectionIdentifier, Guid messageId, CancellationToken cancellationToken){
            Result result = await ApplySMSUpdates(async (SMSAggregate smsAggregate) => {
                // re-send message to provider (record event)
                Result stateResult = smsAggregate.ResendRequestToProvider();
                if (!stateResult.IsSuccess())
                    return stateResult;

                // Make call to SMS provider here
                SMSServiceProxyResponse smsResponse =
                    await this.SmsServiceProxy.SendSMS(messageId, smsAggregate.Sender, smsAggregate.Destination, smsAggregate.Message, cancellationToken);

                // response message from provider (record event)
                stateResult = smsAggregate.ReceiveResponseFromProvider(smsResponse.SMSIdentifier);
                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, messageId, cancellationToken);
            return result;
        }

        public async Task<Result> UpdateMessageStatus(EmailCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) {
            Result result = await ApplyEmailUpdates(async (EmailAggregate emailAggregate) => {

                Result stateResult;

                switch (command.Status) {
                    case EmailServices.MessageStatus.Bounced:
                        stateResult = emailAggregate.MarkMessageAsBounced(command.Description, command.Timestamp);
                        break;
                    case EmailServices.MessageStatus.Delivered:
                        stateResult = emailAggregate.MarkMessageAsDelivered(command.Description, command.Timestamp);
                        break;
                    case EmailServices.MessageStatus.Failed:
                    case EmailServices.MessageStatus.Unknown:
                        stateResult = emailAggregate.MarkMessageAsFailed(command.Description, command.Timestamp);
                        break;
                    case EmailServices.MessageStatus.Rejected:
                        stateResult = emailAggregate.MarkMessageAsRejected(command.Description, command.Timestamp);
                        break;
                    case EmailServices.MessageStatus.Spam:
                        stateResult = emailAggregate.MarkMessageAsSpam(command.Description, command.Timestamp);
                        break;
                    default:
                        // Unknown status - treat as failed
                        stateResult = emailAggregate.MarkMessageAsFailed(command.Description, command.Timestamp);
                        break;
                }

                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, command.MessageId, cancellationToken);
            return result;
        }

        public async Task<Result> UpdateMessageStatus(SMSCommands.UpdateMessageStatusCommand command,
                                                      CancellationToken cancellationToken) {
            Result result = await ApplySMSUpdates(async (SMSAggregate smsAggregate) => {
                Result stateResult;

                switch (command.Status) {
                    case SMSServices.MessageStatus.Delivered:
                    case SMSServices.MessageStatus.Sent:
                    case SMSServices.MessageStatus.InProgress:
                        stateResult = smsAggregate.MarkMessageAsDelivered(command.Description, command.Timestamp);
                        break;
                    case SMSServices.MessageStatus.Expired:
                        stateResult = smsAggregate.MarkMessageAsExpired(command.Description, command.Timestamp);
                        break;
                    case SMSServices.MessageStatus.Rejected:
                        stateResult = smsAggregate.MarkMessageAsRejected(command.Description, command.Timestamp);
                        break;
                    case SMSServices.MessageStatus.Undeliverable:
                        stateResult = smsAggregate.MarkMessageAsUndeliverable(command.Description, command.Timestamp);
                        break;
                    default:
                        stateResult = smsAggregate.MarkMessageAsRejected(command.Description, command.Timestamp);
                        break;
                }
                if (!stateResult.IsSuccess())
                    return stateResult;

                return Results.Ok();
            }, command.MessageId, cancellationToken);
            return result;
        }
    }

    public static class DomainServiceHelper
    {
        public static Result<T> HandleGetAggregateResult<T>(Result<T> result, Guid aggregateId, bool isNotFoundError = true)
            where T : Aggregate, new()  // Constraint: T is a subclass of Aggregate and has a parameterless constructor
        {
            if (!result.IsSuccess())
            {
                bool isNotFound = result.GetErrors().All(error => error.ErrorType == ErrorType.NotFound);
                if (!isNotFound || isNotFoundError)
                {
                    return result;
                }

                return Results.Ok(new T { AggregateId = aggregateId });
            }

            return result;
        }
    }

    #endregion
}
