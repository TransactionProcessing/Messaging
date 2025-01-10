namespace MessagingService.Testing
{
    using MessagingService.BusinessLogic.Requests;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using BusinessLogic.Services.EmailServices;
    using BusinessLogic.Services.SMSServices;
    using EmailMessage.DomainEvents;
    using EmailMessageAggregate;
    using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
    using Shared.DomainDrivenDesign.EventSourcing;
    using SMSMessage.DomainEvents;
    using SMSMessageAggregate;
    using EmailMessageStatus = BusinessLogic.Services.EmailServices.MessageStatus;
    using SMSMessageStatus = BusinessLogic.Services.SMSServices.MessageStatus;
    using EmailMessageStatusResponse = BusinessLogic.Services.EmailServices.MessageStatusResponse;
    using SMSMessageStatusResponse = BusinessLogic.Services.SMSServices.MessageStatusResponse;
    using Microsoft.Extensions.Configuration;
    using static MessagingService.BusinessLogic.Requests.EmailCommands;

    public class TestData
    {
        public static Guid ConnectionIdentifier = Guid.Parse("AF987E74-B01F-456D-8182-ECCB82CBB5B1");

        public static Guid MessageId = Guid.Parse("DB41ED46-0F4A-4995-B497-E62D0223F66B");

        public static String FromAddress = "testfromemail@email.com";

        public static List<String> ToAddresses = new List<String>
                                                 {
                                                     "testtoemail1@email.com",
                                                     "testtoemail2@email.com"
                                                 };

        public static String Subject = "Test Subject";

        public static String Body = "Test Body";

        public static Boolean IsHtmlTrue = true;

        public static String EmailIdentifier = "36B1021A-3668-4C47-9949-8C8BF4AA041D";

        public static HttpStatusCode ApiStatusCodeSuccess = HttpStatusCode.OK;

        public static String RequestIdentifier = "E0FEFE04-178D-492B-B5D2-1C22189D88B3";

        public static String ProviderRequestReference = "ProviderRequestReference";

        public static String ProviderEmailReference = "ProviderEmailReference";

        public static EmailMessageStatus EmailMessageStatusDelivered = EmailMessageStatus.Delivered;

        public static EmailMessageStatus EmailMessageStatusRejected = EmailMessageStatus.Rejected;
        
        public static EmailMessageStatus EmailMessageStatusFailed = EmailMessageStatus.Failed;
        
        public static EmailMessageStatus EmailMessageStatusBounced = EmailMessageStatus.Bounced;
        
        public static EmailMessageStatus EmailMessageStatusSpam = EmailMessageStatus.Spam;
        
        public static EmailMessageStatus EmailMessageStatusUnknown = EmailMessageStatus.Unknown;

        public static SMSMessageStatus SMSMessageStatusExpired = SMSMessageStatus.Expired;
        public static SMSMessageStatus SMSMessageStatusIncoming = SMSMessageStatus.Incoming;
        public static SMSMessageStatus SMSMessageStatusUndeliverable = SMSMessageStatus.Undeliverable;
        public static SMSMessageStatus SMSMessageStatusDelivered = SMSMessageStatus.Delivered;
        public static SMSMessageStatus SMSMessageStatusRejected = SMSMessageStatus.Rejected;

        public static HttpStatusCode ProviderApiStatusCode = HttpStatusCode.OK;

        public static String ProviderStatusDescription = "delivered";

        public static DateTime DeliveredDateTime = DateTime.Now;

        public static DateTime RejectedDateTime = DateTime.Now;

        public static DateTime BouncedDateTime = DateTime.Now;

        public static DateTime FailedDateTime = DateTime.Now;

        public static DateTime SpamDateTime = DateTime.Now;

        public static String Sender = "Sender";
        public static String Destination = "07777777770";

        public static String Message = "Test SMS Message";

        public static String ProviderSMSReference = "SMSReference";

        public static DateTime UndeliveredDateTime = DateTime.Now;

        public static DateTime ExpiredDateTime = DateTime.Now;

        public static SMSCommands.SendSMSCommand SendSMSCommand = new(TestData.ConnectionIdentifier,
                                                                            TestData.MessageId,
                                                                            TestData.Sender,
                                                                            TestData.Destination,
                                                                            TestData.Message);

        private static String SMSIdentifier = "2FACEDE5-0915-46A2-B1EE-CD904746DDD0";

        public static EmailMessageStatusResponse MessageStatusResponseDelivered =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusDelivered
            };

        public static SMSMessageStatusResponse SMSMessageStatusResponseDelivered =>
            new SMSMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.SMSMessageStatusDelivered
            };

        public static SMSMessageStatusResponse SMSMessageStatusResponseExpired =>
            new SMSMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.SMSMessageStatusExpired
            };

        public static SMSMessageStatusResponse SMSMessageStatusResponseRejected =>
            new SMSMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.SMSMessageStatusRejected
            };

        public static SMSMessageStatusResponse SMSMessageStatusResponseUndelivered =>
            new SMSMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.SMSMessageStatusUndeliverable
            };

        public static EmailMessageStatusResponse MessageStatusResponseBounced =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusBounced
            };

        public static EmailMessageStatusResponse MessageStatusResponseFailed =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusFailed
            };

        public static EmailMessageStatusResponse MessageStatusResponseRejected =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusRejected
            };

        public static EmailMessageStatusResponse MessageStatusResponseSpam =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusSpam
            };

        public static EmailMessageStatusResponse MessageStatusResponseUnknown =>
            new EmailMessageStatusResponse
            {
                ProviderStatusDescription = TestData.ProviderStatusDescription,
                ApiStatusCode = TestData.ProviderApiStatusCode,
                MessageStatus = TestData.EmailMessageStatusUnknown
            };

        public static EmailServiceProxyResponse SuccessfulEmailServiceProxyResponse =>
            new EmailServiceProxyResponse
            {
                EmailIdentifier = TestData.EmailIdentifier,
                ApiCallSuccessful = true,
                RequestIdentifier = TestData.RequestIdentifier
            };

        public static EmailServiceProxyResponse FailedAPICallEmailServiceProxyResponse =>
            new EmailServiceProxyResponse
            {
                Error = "NotFound",
                ErrorCode = "404",
                ApiCallSuccessful = false,
            };

        public static EmailServiceProxyResponse FailedEmailServiceProxyResponse =>
            new EmailServiceProxyResponse
            {
                Error = "NotFound",
                ErrorCode = "404",
                ApiCallSuccessful = true,
            };


        public static List<EmailAttachment> EmptyEmailAttachments => new List<EmailAttachment>();

        public static List<EmailAttachment> EmailAttachments => new List<EmailAttachment>
        {
            new EmailAttachment
            {
                FileData  = TestData.FileData,
                Filename = TestData.FileName,
                FileType = TestData.FileTypePDF
            },
            new EmailAttachment
            {
                FileData  = TestData.FileData,
                Filename = TestData.FileName,
                FileType = TestData.FileTypeNone
            }
        };

        public static List<Models.EmailAttachment> EmailAttachmentModels => new List<Models.EmailAttachment>
                                                                {
                                                                    new Models.EmailAttachment
                                                                    {
                                                                        FileData  = TestData.FileData,
                                                                        Filename = TestData.FileName,
                                                                        FileType = TestData.ModelFileTypePDF
                                                                    }
                                                                };

        public static String FileData = "Base64String";
        public static String FileName = "FileName.pdf";
        public static FileType FileTypeNone = FileType.None;
        public static FileType FileTypePDF = FileType.PDF;
        public static Models.FileType ModelFileTypePDF = Models.FileType.PDF;
        public static Models.FileType ModelFileTypeNone = Models.FileType.None;

        public static String EmailErrorCode = "404";
        public static String EmailError = "NotFound";

        public static SMSCommands.UpdateMessageStatusCommand UpdateSMSMessageStatusCommand =>
            new (TestData.MessageId, SMSMessageStatusDelivered, TestData.ProviderStatusDescription, TestData.DeliveredDateTime);
        public static EmailCommands.UpdateMessageStatusCommand UpdateEmailMessageStatusCommand=>
            new (TestData.MessageId, TestData.EmailMessageStatusDelivered, TestData.ProviderStatusDescription, TestData.DeliveredDateTime);

        public static EmailCommands.ResendEmailCommand ResendEmailCommand => new EmailCommands.ResendEmailCommand(TestData.ConnectionIdentifier, TestData.MessageId);

        public static SMSCommands.ResendSMSCommand ResendSMSCommand => new(TestData.ConnectionIdentifier, TestData.MessageId);

        public static EmailCommands.SendEmailCommand SendEmailCommand => new SendEmailCommand(TestData.ConnectionIdentifier,
                                                                                                TestData.MessageId,
                                                                                                TestData.FromAddress,
                                                                                                TestData.ToAddresses,
                                                                                                TestData.Subject,
                                                                                                TestData.Body,
                                                                                                TestData.IsHtmlTrue,
                                                                                                TestData.EmptyEmailAttachments);
        
        public static EmailCommands.SendEmailCommand SendEmailCommandWithAttachments => new SendEmailCommand(TestData.ConnectionIdentifier,
                                                                                   TestData.MessageId,
                                                                                   TestData.FromAddress,
                                                                                   TestData.ToAddresses,
                                                                                   TestData.Subject,
                                                                                   TestData.Body,
                                                                                   TestData.IsHtmlTrue,
                                                                                   TestData.EmailAttachments);

        public static EmailAggregate GetEmptyEmailAggregate()
        {
            EmailAggregate emailAggregate = new EmailAggregate();

            return emailAggregate;
        }

        public static EmailAggregate GetSentEmailAggregate()
        {
            EmailAggregate emailAggregate = new EmailAggregate();
            emailAggregate.SendRequestToProvider(TestData.FromAddress, TestData.ToAddresses, TestData.Subject,
                                                 TestData.Body, TestData.IsHtmlTrue, TestData.EmailAttachmentModels);
            emailAggregate.ReceiveResponseFromProvider(TestData.ProviderRequestReference, TestData.ProviderEmailReference);
            return emailAggregate;
        }

        public static ResponseReceivedFromEmailProviderEvent ResponseReceivedFromEmailProviderEvent =>
            new(TestData.MessageId, TestData.ProviderRequestReference, TestData.ProviderEmailReference);

        public static ResponseReceivedFromSMSProviderEvent ResponseReceivedFromSMSProviderEvent =>
            new ResponseReceivedFromSMSProviderEvent(TestData.MessageId, TestData.ProviderSMSReference);

        public static SMSServiceProxyResponse SuccessfulSMSServiceProxyResponse =>
            new SMSServiceProxyResponse
            {
                SMSIdentifier = TestData.SMSIdentifier,
                ApiCallSuccessful = true
            };
        

        public static SMSServiceProxyResponse FailedAPICallSMSServiceProxyResponse =>
            new SMSServiceProxyResponse
            {
                Error = "NotFound",
                ErrorCode = "404",
                ApiCallSuccessful = false,
            };

        public static SMSServiceProxyResponse FailedSMSServiceProxyResponse =>
            new SMSServiceProxyResponse
            {
                Error = "NotFound",
                ErrorCode = "404",
                ApiCallSuccessful = true,
            };

        public static SMSAggregate GetEmptySMSAggregate()
        {
            SMSAggregate smsAggregate = new SMSAggregate();

            return smsAggregate;
        }

        public static SMSAggregate GetSentSMSAggregate()
        {
            SMSAggregate smsAggregate = new SMSAggregate();
            smsAggregate.SendRequestToProvider(TestData.Sender, TestData.Destination, TestData.Message);
            smsAggregate.ReceiveResponseFromProvider(TestData.ProviderSMSReference);
            return smsAggregate;
        }

        public static Dictionary<String,String> GetStandardMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            IConfigurationBuilder builder = new ConfigurationBuilder();

            configuration.Add("AppSettings:EmailProxy", "UnitTest");
            configuration.Add("AppSettings:SMSProxy", "UnitTest");
            configuration.Add("ConnectionStrings:HealthCheck", "HeathCheckConnString");
            configuration.Add("SecurityConfiguration:Authority", "https://127.0.0.1");
            configuration.Add("EventStoreSettings:ConnectionString", "esdb://127.0.0.1:2113");
            configuration.Add("EventStoreSettings:ConnectionName", "UnitTestConnection");
            configuration.Add("EventStoreSettings:UserName", "admin");
            configuration.Add("EventStoreSettings:Password", "changeit");
            configuration.Add("AppSettings:UseConnectionStringConfig", "false");
            configuration.Add("AppSettings:SecurityService", "http://127.0.0.1");
            configuration.Add("AppSettings:MessagingServiceApi", "http://127.0.0.1");

            return configuration;
        }
    }

    public record TestEvent : DomainEvent
    {
        public TestEvent(Guid aggregateId,
                 Guid eventId) : base(aggregateId, eventId) {
        }
    }
}