namespace MessagingService.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>Describes a file attached to an email.</summary>
    [ExcludeFromCodeCoverage]
    public class EmailAttachment
    {
        /// <summary>Gets or sets the attachment contents encoded as a Base64 string.</summary>
        public String FileData { get; set; }

        /// <summary>Gets or sets the attachment filename.</summary>
        public String Filename { get; set; }

        /// <summary>Gets or sets the attachment file type.</summary>
        public FileType FileType { get; set; }
    }
}
