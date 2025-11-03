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
            .WithStandardProduces<SendSMSResponse, ErrorResponse>(StatusCodes.Status201Created);

        smsGroup.MapPost("/resend", SMSHandlers.ResendSMS)
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
            .WithName("Resend SMS")
            .WithSummary("Resends an SMS message by id")
            .WithStandardProduces< ErrorResponse>(StatusCodes.Status202Accepted);
            
        return endpoints;
    }
}