namespace MessagingService.DataTransferObjects
{
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>Specifies the supported email attachment file types.</summary>
    public enum FileType
    {
        /// <summary>No recognized file type.</summary>
        None = 0,

        /// <summary>Portable Document Format (PDF).</summary>
        PDF
    }
}
