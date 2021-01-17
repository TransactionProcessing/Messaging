namespace MessagingService.BusinessLogic.Services.SMSServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    [ExcludeFromCodeCoverage]
    public class SMSServiceProxyResponse
    {
        /// <summary>
        /// Gets or sets the API status code.
        /// </summary>
        /// <value>
        /// The API status code.
        /// </value>
        public HttpStatusCode ApiStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the email identifier.
        /// </summary>
        /// <value>
        /// The email identifier.
        /// </value>
        public String SMSIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public String ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public String Error { get; set; }
    }
}