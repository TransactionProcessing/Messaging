using MessagingService.DataTransferObjects;
using MessagingService.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using Shared.Extensions;
using Shared.Middleware;

namespace MessagingService.Endpoints;

public static class SMSEndpoints
{
    private const string BaseRoute = "/api/sms";
    public static IEndpointRouteBuilder MapSMSEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder smsGroup = endpoints.MapGroup(BaseRoute).WithTags("SMS");
            
        smsGroup.MapPost("/", SMSHandlers.SendSMS)
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
            .WithName("Send SMS")
            .WithSummary("Sends an SMS message")
            .WithDescription("Submits an SMS message for delivery. If messageId is omitted or null, the service generates a message identifier.")
            .WithStandardProduces<SendSMSResponse, ErrorResponse>(StatusCodes.Status201Created);

        smsGroup.MapPost("/resend", SMSHandlers.ResendSMS)
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
            .WithName("Resend SMS")
            .WithSummary("Resends an SMS message by id")
            .WithDescription("Requests delivery of an existing SMS message, identified by messageId and connectionIdentifier.")
            .WithStandardProduces< ErrorResponse>(StatusCodes.Status202Accepted);
            
        return endpoints;
    }
}
