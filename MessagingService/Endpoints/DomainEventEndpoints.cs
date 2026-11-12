using MessagingService.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TransactionProcessing.UnionResults.AspNetCore;

namespace MessagingService.Endpoints;

public static class DomainEventEndpoints
{
    private const string Route = "api/domainevents";

    public static void MapDomainEventEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/").AddUnionResultFilter().MapPost(Route, DomainEventHandlers.HandleDomainEvent).ProducesUnionResult()
            .ExcludeFromDescription() // hides from Swagger
            .WithName("DomainEvent");
    }
}