namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class EmailDetails
    {
        public String Subject { get; set; }

        public DateTime DeliveredAt { get; set; }

        public DateTime EmailStatusDate { get; set; }

        public String ProcessStatus { get; set; }

        public String EmailId { get; set; }

        public String Status { get; set; }
    }
}