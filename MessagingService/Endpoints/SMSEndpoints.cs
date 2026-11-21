using MessagingService.DataTransferObjects;
using MessagingService.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using TransactionProcessing.Shared.Authorisation;
using TransactionProcessing.UnionResults.AspNetCore;

namespace MessagingService.Endpoints;

public static class SMSEndpoints
{
    private const string BaseRoute = "/api/sms";
    public static IEndpointRouteBuilder MapSMSEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder smsGroup = endpoints.MapGroup(BaseRoute).WithTags("SMS").AddUnionResultFilter();
            
        smsGroup.MapPost("/", SMSHandlers.SendSMS)
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
            .WithName("Send SMS")
            .WithSummary("Sends an SMS message")
            .WithDescription("Submits an SMS message for delivery. If messageId is omitted or null, the service generates a message identifier.")
            .Produces<SendSMSResponse>(StatusCodes.Status201Created)
            .ProducesMessagingErrors();

        smsGroup.MapPost("/resend", SMSHandlers.ResendSMS)
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
            .WithName("Resend SMS")
            .WithSummary("Resends an SMS message by id")
            .WithDescription("Requests delivery of an existing SMS message, identified by messageId and connectionIdentifier.")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesMessagingErrors();
            
        return endpoints;
    }
}
