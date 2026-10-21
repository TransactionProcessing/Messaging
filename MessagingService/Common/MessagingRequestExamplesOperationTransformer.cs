using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MessagingService.Common;

/// <summary>
/// Adds representative JSON request examples to the messaging endpoints.
/// </summary>
internal sealed class MessagingRequestExamplesOperationTransformer : IOpenApiOperationTransformer
{
    private const string SendEmailExample = """
        {
          "messageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
          "body": "Your appointment is confirmed for tomorrow at 10:00.",
          "connectionIdentifier": "a8098c1a-f86e-11da-bd1a-00112444be1e",
          "fromAddress": "notifications@example.test",
          "isHtml": false,
          "subject": "Appointment confirmed",
          "toAddresses": ["recipient@example.test"],
          "emailAttachments": []
        }
        """;

    private const string ResendEmailExample = """
        {
          "messageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
          "connectionIdentifier": "a8098c1a-f86e-11da-bd1a-00112444be1e"
        }
        """;

    private const string SendSmsExample = """
        {
          "connectionIdentifier": "a8098c1a-f86e-11da-bd1a-00112444be1e",
          "destination": "+447700900123",
          "message": "Your appointment is confirmed for tomorrow at 10:00.",
          "messageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
          "sender": "Example Service"
        }
        """;

    private const string ResendSmsExample = """
        {
          "messageId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
          "connectionIdentifier": "a8098c1a-f86e-11da-bd1a-00112444be1e"
        }
        """;

    public Task TransformAsync(OpenApiOperation operation,
                               OpenApiOperationTransformerContext context,
                               CancellationToken cancellationToken)
    {
        string path = context.Description.RelativePath?.Trim('/') ?? string.Empty;
        string? example = (context.Description.HttpMethod, path) switch
        {
            ("POST", "api/email") => SendEmailExample,
            ("POST", "api/email/resend") => ResendEmailExample,
            ("POST", "api/sms") => SendSmsExample,
            ("POST", "api/sms/resend") => ResendSmsExample,
            _ => null
        };

        if (example is not null
            && operation.RequestBody is OpenApiRequestBody requestBody
            && requestBody.Content.TryGetValue("application/json", out OpenApiMediaType? mediaType))
        {
            mediaType.Example = JsonNode.Parse(example);
        }

        return Task.CompletedTask;
    }
}
