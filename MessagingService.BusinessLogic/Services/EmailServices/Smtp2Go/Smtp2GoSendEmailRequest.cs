using System;
using Newtonsoft.Json;

namespace MessagingService.Service.Services.Email.Smtp2Go
{
    public class Smtp2GoSendEmailRequest
    {
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        [JsonProperty("api_key")]
        public String ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [test mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("test")]
        public Boolean TestMode { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [JsonProperty("sender")]
        public String Sender { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        [JsonProperty("to")]
        public String[] To { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        [JsonProperty("cc")]
        public String[] CC { get; set; }

        /// <summary>
        /// Gets or sets the BCC.
        /// </summary>
        /// <value>
        /// The BCC.
        /// </value>
        [JsonProperty("bcc")]
        public String[] BCC { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonProperty("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body.
        /// </summary>
        /// <value>
        /// The HTML body.
        /// </value>
        [JsonProperty("html_body")]
        public String HTMLBody { get; set; }

        /// <summary>
        /// Gets or sets the text body.
        /// </summary>
        /// <value>
        /// The text body.
        /// </value>
        [JsonProperty("text_body")]
        public String TextBody { get; set; }
    }
}