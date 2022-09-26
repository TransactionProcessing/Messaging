namespace MessagingService.BusinessLogic.Requests;

using System;

public class EmailAttachment
{
    public String Filename { get; set; }

    public String FileData { get; set; }

    public FileType FileType { get; set; }
}