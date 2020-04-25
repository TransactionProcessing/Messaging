namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailServiceProxyResponse
    {
        /// <summary>
        /// Gets or sets the API status code.
        /// </summary>
        /// <value>
        /// The API status code.
        /// </value>
        public HttpStatusCode ApiStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public String RequestIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the email identifier.
        /// </summary>
        /// <value>
        /// The email identifier.
        /// </value>
        public String EmailIdentifier { get; set; }

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