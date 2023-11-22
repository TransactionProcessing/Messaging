namespace MessagingService.IntegrationTesting.Helpers;

using MessagingService.DataTransferObjects;
using Shared.IntegrationTesting;
using TechTalk.SpecFlow;

public static class SpecflowExtensions{
    public static List<SendEmailRequest> ToSendEmailRequests(this TableRows tableRows){
        List<SendEmailRequest> requests = new List<SendEmailRequest>();
        foreach (TableRow tableRow in tableRows){
            String fromAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "FromAddress");
            String toAddresses = SpecflowTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            String subject = SpecflowTableHelper.GetStringRowValue(tableRow, "Subject");
            String body = SpecflowTableHelper.GetStringRowValue(tableRow, "Body");
            Boolean isHtml = SpecflowTableHelper.GetBooleanValue(tableRow, "IsHtml");

            SendEmailRequest request = new SendEmailRequest
                                       {
                                           Body = body,
                                           ConnectionIdentifier = Guid.NewGuid(),
                                           FromAddress = fromAddress,
                                           IsHtml = isHtml,
                                           Subject = subject,
                                           ToAddresses = toAddresses.Split(",").ToList()
                                       };
            requests.Add(request);
        }

        return requests;
    }

    public static List<ResendEmailRequest> ToResendEmailRequests(this TableRows tableRows, Dictionary<String, SendEmailResponse> sendResponses)
    {
        List<ResendEmailRequest> requests = new List<ResendEmailRequest>();

        foreach (TableRow tableRow in tableRows){
            String toAddresses = SpecflowTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            SendEmailResponse sendEmailResponse = sendResponses[toAddresses];

            ResendEmailRequest request = new ResendEmailRequest()
                                         {
                                             ConnectionIdentifier = Guid.NewGuid(),
                                             MessageId = sendEmailResponse.MessageId
                                         };
            requests.Add(request);
        }
        return requests;
    }

    public static List<SendSMSRequest> ToSendSMSRequests(this TableRows tableRows){
        List<SendSMSRequest> requests = new List<SendSMSRequest>();

        foreach (TableRow tableRow in tableRows){
            String sender = SpecflowTableHelper.GetStringRowValue(tableRow, "Sender");
            String destination = SpecflowTableHelper.GetStringRowValue(tableRow, "Destination");
            String message = SpecflowTableHelper.GetStringRowValue(tableRow, "Message");

            SendSMSRequest request = new SendSMSRequest
                                     {
                                         ConnectionIdentifier = Guid.NewGuid(),
                                         Sender = sender,
                                         Destination = destination,
                                         Message = message
                                     };
            requests.Add(request);
        }
        return requests;
    }
}