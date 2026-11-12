using MessagingService.DataTransferObjects;
using MessagingService.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using TransactionProcessing.Shared.Authorisation;
using TransactionProcessing.UnionResults.AspNetCore;

namespace MessagingService.Endpoints
{
    public static class EmailEndpoints
    {
        private const string BaseRoute = "/api/email";

        public static IEndpointRouteBuilder MapEmailEndpoints(this IEndpointRouteBuilder endpoints)
        {
            RouteGroupBuilder emailGroup = endpoints.MapGroup(BaseRoute).WithTags("Email").AddUnionResultFilter();

            emailGroup.MapPost("/", EmailHandlers.SendEmail)
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
                .WithName("Send Email")
                .WithSummary("Sends an email")
                .WithDescription("Submits an email for delivery. If messageId is omitted or null, the service generates a message identifier. Attachments may be included in the request.")
                .ProducesUnionResult<SendEmailResponse>();

            emailGroup.MapPost("/resend", EmailHandlers.ResendEmail)
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.ClientCredentialsOnlyPolicy)
                .WithName("Resend Email")
                .WithSummary("Resends an email by id")
                .WithDescription("Requests delivery of an existing email message, identified by messageId and connectionIdentifier.")
                .ProducesUnionResult()
                .Produces(StatusCodes.Status202Accepted);

            return endpoints;
        }
    }
}
