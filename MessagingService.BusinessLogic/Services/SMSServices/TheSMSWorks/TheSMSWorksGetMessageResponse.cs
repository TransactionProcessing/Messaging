namespace MessagingService.BusinessLogic.Services.SMSServices.TheSMSWorks
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TheSMSWorksGetMessageResponse
    {
        public String BatchId { get; set; }

        public String Content { get; set; }

        public String Created { get; set; }

        public String Customerid { get; set; }

        public String Deliveryreporturl { get; set; }

        public String Destination { get; set; }

        public TheSMSWorksFailureReason Failurereason { get; set; }

        public String Id { get; set; }

        public String Identifier { get; set; }

        public String Keyword { get; set; }

        public String Messageid { get; set; }

        public String Modified { get; set; }

        public String Schedule { get; set; }

        public String Status { get; set; }
        public String Sender { get; set; }

        public String Tag { get; set; }
    }
}