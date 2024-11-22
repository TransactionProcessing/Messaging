using Shared.Results;
using SimpleResults;

namespace MessagingService.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;
    using EmailMessage.DomainEvents;
    using EmailMessageAggregate;
    using EmailServices;
    using Microsoft.Extensions.Logging;
    using Models;
    using Shared.DomainDrivenDesign.EventSourcing;
    using Shared.EventStore.Aggregate;
    using Shared.Exceptions;
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

                EmailAggregate emailAggregate = emailAggregateResult.Data;
                Result result = await action(emailAggregate);
                if (result.IsFailed)
                    return ResultHelpers.CreateFailure(result);

                Result saveResult = await this.EmailAggregateRepository.SaveChanges(emailAggregate, cancellationToken);
                if (saveResult.IsFailed)
                    return ResultHelpers.CreateFailure(saveResult);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.GetExceptionMessages());
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

                SMSAggregate smsAggregate = smsAggregateResult.Data;
                Result result = await action(smsAggregate);
                if (result.IsFailed)
                    return ResultHelpers.CreateFailure(result);

                Result saveResult = await this.SmsAggregateRepository.SaveChanges(smsAggregate, cancellationToken);
                if (saveResult.IsFailed)
                    return ResultHelpers.CreateFailure(saveResult);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.GetExceptionMessages());
            }
        }

        public async Task<Result> SendEmailMessage(Guid connectionIdentifier,
                                                   Guid messageId,
                                                   String fromAddress,
                                                   List<String> toAddresses,
                                                   String subject,
                                                   String body,
                                                   Boolean isHtml,
                                                   List<EmailAttachment> attachments,
                                                   CancellationToken cancellationToken) {
            Result result = await ApplyEmailUpdates(async (EmailAggregate emailAggregate) => {
                // send message to provider (record event)
                emailAggregate.SendRequestToProvider(fromAddress, toAddresses, subject, body, isHtml, attachments);

                // Make call to Email provider here
                EmailServiceProxyResponse emailResponse = await this.EmailServiceProxy.SendEmail(messageId, fromAddress,
                    toAddresses, subject, body, isHtml, attachments, cancellationToken);

                if (emailResponse.ApiCallSuccessful) {
                    // response message from provider (record event)
                    emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier,
                        emailResponse.EmailIdentifier);
                }
                else {
                    emailAggregate.ReceiveBadResponseFromProvider(emailResponse.Error, emailResponse.ErrorCode);
                }

                return Result.Success();
            }, messageId, cancellationToken,false);
            return result;
        }

        public async Task<Result> SendSMSMessage(Guid connectionIdentifier,
                                                 Guid messageId,
                                                 String sender,
                                                 String destination,
                                                 String message,
                                                 CancellationToken cancellationToken)
        {
            Result result = await ApplySMSUpdates(async (SMSAggregate smsAggregate) => {
                // send message to provider (record event)
                smsAggregate.SendRequestToProvider(sender, destination, message);

                // Make call to SMS provider here
                SMSServiceProxyResponse smsResponse =
                    await this.SmsServiceProxy.SendSMS(messageId, sender, destination, message, cancellationToken);

                // response message from provider (record event)
                smsAggregate.ReceiveResponseFromProvider(smsResponse.SMSIdentifier);

                return Result.Success();
            }, messageId, cancellationToken, false);
            return result;
        }

        public async Task<Result> ResendEmailMessage(Guid connectionIdentifier,
                                                     Guid messageId,
                                                     CancellationToken cancellationToken) {

            Result result = await ApplyEmailUpdates(async (EmailAggregate emailAggregate) => {
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

                if (emailResponse.ApiCallSuccessful)
                {
                    // response message from provider (record event)
                    emailAggregate.ReceiveResponseFromProvider(emailResponse.RequestIdentifier,
                        emailResponse.EmailIdentifier);
                }
                else
                {
                    emailAggregate.ReceiveBadResponseFromProvider(emailResponse.Error, emailResponse.ErrorCode);
                }

                return Result.Success();
            }, messageId, cancellationToken);
            return result;
        }

        public async Task<Result> ResendSMSMessage(Guid connectionIdentifier, Guid messageId, CancellationToken cancellationToken){
            Result result = await ApplySMSUpdates(async (SMSAggregate smsAggregate) => {
                // re-send message to provider (record event)
                smsAggregate.ResendRequestToProvider();

                // Make call to SMS provider here
                SMSServiceProxyResponse smsResponse =
                    await this.SmsServiceProxy.SendSMS(messageId, smsAggregate.Sender, smsAggregate.Destination, smsAggregate.Message, cancellationToken);

                // response message from provider (record event)
                smsAggregate.ReceiveResponseFromProvider(smsResponse.SMSIdentifier);

                return Result.Success();
            }, messageId, cancellationToken);
            return result;
        }
    }

    public static class DomainServiceHelper
    {
        public static Result<T> HandleGetAggregateResult<T>(Result<T> result, Guid aggregateId, bool isNotFoundError = true)
            where T : Aggregate, new()  // Constraint: T is a subclass of Aggregate and has a parameterless constructor
        {
            if (result.IsFailed && result.Status != ResultStatus.NotFound)
            {
                return ResultHelpers.CreateFailure(result);
            }

            if (result.Status == ResultStatus.NotFound && isNotFoundError)
            {
                return ResultHelpers.CreateFailure(result);
            }

            T aggregate = result.Status switch
            {
                ResultStatus.NotFound => new T { AggregateId = aggregateId },  // Set AggregateId when creating a new instance
                _ => result.Data
            };

            return Result.Success(aggregate);
        }
    }

    #endregion
}