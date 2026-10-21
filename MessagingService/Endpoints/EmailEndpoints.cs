using MessagingService.DataTransferObjects;
using MessagingService.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using Shared.Extensions;
using Shared.Middleware;

namespace MessagingService.Endpoints
{
    public static class EmailEndpoints
    {
        private const string BaseRoute = "/api/email";

        public static IEndpointRouteBuilder MapEmailEndpoints(this IEndpointRouteBuilder endpoints)
        {
            RouteGroupBuilder emailGroup = endpoints.MapGroup(BaseRoute).WithTags("Email");

            emailGroup.MapPost("/", EmailHandlers.SendEmail)
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
                .WithName("Send Email")
                .WithSummary("Sends an email")
                .WithDescription("Submits an email for delivery. If messageId is omitted or null, the service generates a message identifier. Attachments may be included in the request.")
                .WithStandardProduces<SendEmailResponse,ErrorResponse>(StatusCodes.Status201Created);

            emailGroup.MapPost("/resend", EmailHandlers.ResendEmail)
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
                .WithName("Resend Email")
                .WithSummary("Resends an email by id")
                .WithDescription("Requests delivery of an existing email message, identified by messageId and connectionIdentifier.")
                .WithStandardProduces< ErrorResponse>(StatusCodes.Status202Accepted);

            return endpoints;
        }
    }
}
