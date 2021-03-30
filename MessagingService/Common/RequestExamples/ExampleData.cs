namespace MessagingService.Common.RequestExamples
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ExampleData
    {
        #region Fields

        /// <summary>
        /// The connection identifier
        /// </summary>
        internal static Guid ConnectionIdentifier = Guid.Parse("A6080219-E243-48F8-A6C3-D79610A74A5B");

        /// <summary>
        /// The email message from address
        /// </summary>
        internal static String EmailMessageFromAddress = "fromaddress@exampleemail.com";

        /// <summary>
        /// The email message HTML body
        /// </summary>
        internal static String EmailMessageHtmlBody = "<p>This is a test message body</p>";

        /// <summary>
        /// The email message identifier
        /// </summary>
        internal static Guid EmailMessageId = Guid.Parse("63BDE20F-28E0-4698-AF46-923A08198994");

        /// <summary>
        /// The email message is HTML
        /// </summary>
        internal static Boolean EmailMessageIsHtml = true;

        /// <summary>
        /// The email message plain text body
        /// </summary>
        internal static String EmailMessagePlainTextBody = "This is a test message body";

        /// <summary>
        /// The email message plain text is HTML
        /// </summary>
        internal static Boolean EmailMessagePlainTextIsHtml = false;

        /// <summary>
        /// The email message subject
        /// </summary>
        internal static String EmailMessageSubject = "Email Subject";

        /// <summary>
        /// The email message to address1
        /// </summary>
        internal static String EmailMessageToAddress1 = "toaddress1@exampleemail.com";

        /// <summary>
        /// The email message to address2
        /// </summary>
        internal static String EmailMessageToAddress2 = "toaddress2@exampleemail.com";

        /// <summary>
        /// The SMS message destination
        /// </summary>
        internal static String SMSMessageDestination = "07123456789";

        /// <summary>
        /// The SMS message identifier
        /// </summary>
        internal static Guid SMSMessageId = Guid.Parse("D38E20B1-64F1-4217-B192-24123862FE10");

        /// <summary>
        /// The SMS message message
        /// </summary>
        internal static String SMSMessageMessage = "Test SMS Message";

        /// <summary>
        /// The SMS message sender
        /// </summary>
        internal static String SMSMessageSender = "07000000001";

        #endregion
    }
}