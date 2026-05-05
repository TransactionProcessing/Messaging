namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailAttachment
    {
        public String FileData { get; set; }

        public String Filename { get; set; }

        public FileType FileType { get; set; }
    }
}