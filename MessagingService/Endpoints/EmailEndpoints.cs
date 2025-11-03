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
                .WithStandardProduces<SendEmailResponse,ErrorResponse>(StatusCodes.Status201Created);

            emailGroup.MapPost("/resend", EmailHandlers.ResendEmail)
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
                .WithName("Resend Email")
                .WithSummary("Resends an email by id")
                .WithStandardProduces< ErrorResponse>(StatusCodes.Status202Accepted);

            return endpoints;
        }
    }
}
