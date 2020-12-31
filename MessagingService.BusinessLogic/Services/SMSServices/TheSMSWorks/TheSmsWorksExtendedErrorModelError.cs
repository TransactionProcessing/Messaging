namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    public class TheSmsWorksExtendedErrorModelError
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonProperty("code")]
        public String Code { get; set; }

        /// <summary>
        /// Gets or sets the field errors.
        /// </summary>
        /// <value>
        /// The field errors.
        /// </value>
        [JsonProperty("errors")]
        public TheSmsWorksExtendedErrorModelFieldError[] FieldErrors { get; set; }

        /// <summary>
        /// Gets or sets the in.
        /// </summary>
        /// <value>
        /// The in.
        /// </value>
        [JsonProperty("in")]
        public String In { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [JsonProperty("path")]
        public String[] Path { get; set; }
    }
}