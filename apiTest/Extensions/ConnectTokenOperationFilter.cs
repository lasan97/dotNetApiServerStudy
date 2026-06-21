using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace apiTest.Extensions;

internal sealed class ConnectTokenOperationFilter : IOperationFilter
{
    private const string TokenEndpoint = "connect/token";
    private const string FormContentType = "application/x-www-form-urlencoded";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!string.Equals(context.ApiDescription.RelativePath, TokenEndpoint, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [FormContentType] = new()
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["grant_type"] = CreateStringSchema("password"),
                            ["client_id"] = CreateStringSchema("internal-first-party"),
                            ["username"] = CreateStringSchema("user@example.com"),
                            ["password"] = CreateStringSchema(),
                            ["scope"] = CreateStringSchema("api offline_access"),
                            ["refresh_token"] = CreateStringSchema()
                        }
                    }
                }
            }
        };
    }

    private static OpenApiSchema CreateStringSchema(string? defaultValue = null)
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.String
        };

        if (defaultValue is not null)
        {
            schema.Default = JsonValue.Create(defaultValue);
        }

        return schema;
    }
}
