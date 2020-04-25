namespace MessagingService.Testing
{
    using MessagingService.BusinessLogic.Requests;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using BusinessLogic.Services.EmailServices;
    using EmailMessageAggregate;
    using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

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

        public static EmailServiceProxyResponse SuccessfulEmailServiceProxyResponse =>
            new EmailServiceProxyResponse
            {
                EmailIdentifier = TestData.EmailIdentifier,
                ApiStatusCode = TestData.ApiStatusCodeSuccess,
                RequestIdentifier = TestData.RequestIdentifier
            };

        public static SendEmailRequest SendEmailRequest => SendEmailRequest.Create(TestData.ConnectionIdentifier,
                                                                                   TestData.MessageId,
                                                                                   TestData.FromAddress,
                                                                                   TestData.ToAddresses,
                                                                                   TestData.Subject,
                                                                                   TestData.Body,
                                                                                   TestData.IsHtmlTrue);

        public static EmailAggregate GetEmptyEmailAggregate()
        {
            EmailAggregate emailAggregate = new EmailAggregate();

            return emailAggregate;
        }
    }
}