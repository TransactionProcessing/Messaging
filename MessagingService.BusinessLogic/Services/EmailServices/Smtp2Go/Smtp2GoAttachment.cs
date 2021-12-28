namespace MessagingService.Service.Services.Email.Smtp2Go
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;


    [ExcludeFromCodeCoverage]
    public class Smtp2GoAttachment
    {
        #region Properties

        [JsonProperty("fileblob")]
        public String FileBlob { get; set; }

        [JsonProperty("filename")]
        public String FileName { get; set; }

        [JsonProperty("mimetype")]
        public String MimeType { get; set; }

        #endregion
    }
}