namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TheSMSWorksFailureReason
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonProperty("code")]
        public Int32 Code { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        [JsonProperty("details")]
        public String Details { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TheSMSWorksFailureReason"/> is permanent.
        /// </summary>
        /// <value>
        ///   <c>true</c> if permanent; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("permanent")]
        public Boolean Permanent { get; set; }
    }
}