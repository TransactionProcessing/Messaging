namespace MessagingService.IntegrationTesting.Helpers;

using MessagingService.DataTransferObjects;
using Reqnroll;
using Shared.IntegrationTesting;

public static class ReqnrollExtensions{
    public static List<SendEmailRequest> ToSendEmailRequests(this DataTableRows tableRows){
        List<SendEmailRequest> requests = new();
        foreach (DataTableRow tableRow in tableRows){
            String fromAddress = ReqnrollTableHelper.GetStringRowValue(tableRow, "FromAddress");
            String toAddresses = ReqnrollTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            String subject = ReqnrollTableHelper.GetStringRowValue(tableRow, "Subject");
            String body = ReqnrollTableHelper.GetStringRowValue(tableRow, "Body");
            Boolean isHtml = ReqnrollTableHelper.GetBooleanValue(tableRow, "IsHtml");

            SendEmailRequest request = new()
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

    public static List<ResendEmailRequest> ToResendEmailRequests(this DataTableRows tableRows, Dictionary<String, SendEmailResponse> sendResponses)
    {
        List<ResendEmailRequest> requests = new();

        foreach (DataTableRow tableRow in tableRows){
            String toAddresses = ReqnrollTableHelper.GetStringRowValue(tableRow, "ToAddresses");
            SendEmailResponse sendEmailResponse = sendResponses[toAddresses];

            ResendEmailRequest request = new()
                                         {
                                             ConnectionIdentifier = Guid.NewGuid(),
                                             MessageId = sendEmailResponse.MessageId
                                         };
            requests.Add(request);
        }
        return requests;
    }

    public static List<SendSMSRequest> ToSendSMSRequests(this DataTableRows tableRows){
        List<SendSMSRequest> requests = new();

        foreach (DataTableRow tableRow in tableRows){
            String sender = ReqnrollTableHelper.GetStringRowValue(tableRow, "Sender");
            String destination = ReqnrollTableHelper.GetStringRowValue(tableRow, "Destination");
            String message = ReqnrollTableHelper.GetStringRowValue(tableRow, "Message");

            SendSMSRequest request = new()
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