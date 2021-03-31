namespace MessagingService.Common.RequestExamples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using DataTransferObjects;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.Filters.IMultipleExamplesProvider{MessagingService.DataTransferObjects.SendEmailRequest}" />
    [ExcludeFromCodeCoverage]
    public class SendEmailRequestExample : IMultipleExamplesProvider<SendEmailRequest>
    {
        #region Methods

        /// <summary>
        /// Gets the examples.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SwaggerExample<SendEmailRequest>> GetExamples()
        {
            SendEmailRequest htmlEmailRequest = new SendEmailRequest
                                                {
                                                    Body = ExampleData.EmailMessageHtmlBody,
                                                    ConnectionIdentifier = ExampleData.ConnectionIdentifier,
                                                    FromAddress = ExampleData.EmailMessageFromAddress,
                                                    ToAddresses = new List<String>
                                                                  {
                                                                      ExampleData.EmailMessageToAddress1,
                                                                      ExampleData.EmailMessageToAddress2
                                                                  },
                                                    IsHtml = ExampleData.EmailMessageIsHtml,
                                                    MessageId = ExampleData.EmailMessageId,
                                                    Subject = ExampleData.EmailMessageSubject
                                                };

            SendEmailRequest plainTextEmailRequest = new SendEmailRequest
                                                     {
                                                         Body = ExampleData.EmailMessagePlainTextBody,
                                                         ConnectionIdentifier = ExampleData.ConnectionIdentifier,
                                                         FromAddress = ExampleData.EmailMessageFromAddress,
                                                         ToAddresses = new List<String>
                                                                       {
                                                                           ExampleData.EmailMessageToAddress1,
                                                                           ExampleData.EmailMessageToAddress2
                                                                       },
                                                         IsHtml = ExampleData.EmailMessagePlainTextIsHtml,
                                                         MessageId = ExampleData.EmailMessageId,
                                                         Subject = ExampleData.EmailMessageSubject
                                                     };
            List<SwaggerExample<SendEmailRequest>> examples = new List<SwaggerExample<SendEmailRequest>>();
            examples.Add(new SwaggerExample<SendEmailRequest>
                         {
                             Name = "Html Email Request",
                             Value = htmlEmailRequest
                         });
            examples.Add(new SwaggerExample<SendEmailRequest>
                         {
                             Name = "Plan Text Email Request",
                             Value = plainTextEmailRequest
                         });
            return examples;
        }

        #endregion
    }
}