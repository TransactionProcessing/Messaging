namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailAttachment
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        [JsonProperty("file_data")]
        public String FileData { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [JsonProperty("file_name")]
        public String Filename { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>
        /// The type of the file.
        /// </value>
        [JsonProperty("file_type")]
        public FileType FileType { get; set; }

        #endregion
    }
}