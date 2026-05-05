namespace MessagingService.Service.Services.Email.Smtp2Go
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class Smtp2GoAttachment
    {
        public String FileBlob { get; set; }

        public String FileName { get; set; }

        public String MimeType { get; set; }
    }
}