namespace MessagingService.BusinessLogic.Services.EmailServices.Smtp2Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Data
    {

        public Int32 Count { get; set; }

        public List<EmailDetails> EmailDetails { get; set; }
    }
}