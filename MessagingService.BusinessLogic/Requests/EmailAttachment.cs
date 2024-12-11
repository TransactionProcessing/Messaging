using System.Diagnostics.CodeAnalysis;

namespace MessagingService.BusinessLogic.Requests;

using System;

[ExcludeFromCodeCoverage]
public class EmailAttachment
{
    public String Filename { get; set; }

    public String FileData { get; set; }

    public FileType FileType { get; set; }
}