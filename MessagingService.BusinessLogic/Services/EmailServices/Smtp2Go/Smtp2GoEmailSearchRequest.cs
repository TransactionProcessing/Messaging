namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class Smtp2GoEmailSearchRequest
    {
        public List<String> EmailId { get; set; }

        public String ApiKey { get; set; }

        public String StartDate { get; set; }

        public String EndDate { get; set; }
    }
}