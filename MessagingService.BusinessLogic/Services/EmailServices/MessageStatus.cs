namespace MessagingService.BusinessLogic.Services.EmailServices
{
    using System.Diagnostics.CodeAnalysis;

    public enum MessageStatus
    {
        Delivered,
        Failed,
        Bounced,
        Rejected,
        Spam,
        Unknown
    }
}