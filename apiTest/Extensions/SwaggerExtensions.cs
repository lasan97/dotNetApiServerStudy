using apiTest.Services.Auth;
using Microsoft.OpenApi;
using OpenIddict.Abstractions;

namespace apiTest.Extensions;

public static class SwaggerExtensions
{
    private const string Oauth2Scheme = "Oauth2";

    // Swagger DI
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(Oauth2Scheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            [AuthConstants.ApiScope] = "API endpoints",
                            [OpenIddictConstants.Scopes.OfflineAccess] = "Token refresh"
                        }
                    }
                }
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(Oauth2Scheme, document)] = [AuthConstants.ApiScope]
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId(AuthConfiguration.GetInternalClientId(app.Configuration));
            options.OAuthScopes(AuthConstants.ApiScope);
        });

        return app;
    }
}
