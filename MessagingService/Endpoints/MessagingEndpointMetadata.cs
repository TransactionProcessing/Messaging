using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using TransactionProcessing.UnionResults.AspNetCore;

namespace MessagingService.Endpoints;

internal static class MessagingEndpointMetadata
{
    public static RouteHandlerBuilder ProducesMessagingErrors(this RouteHandlerBuilder endpoint)
    {
        return endpoint.ProducesUnionResult().AddOpenApiOperationTransformer(static (operation, _, _) =>
        {
            operation.Responses?.Remove("200");
            return Task.CompletedTask;
        });
    }
}
