using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Services.Auth;

public sealed class OpenIddictApplicationSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(AuthConstants.InternalClientId, cancellationToken) is not null)
        {
            return;
        }

        // password grant는 내부 first-party client에만 허용
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = AuthConstants.InternalClientId,
            ClientType = ClientTypes.Public,
            DisplayName = "Internal first-party client",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthConstants.ApiScope,
                Permissions.Prefixes.Scope + Scopes.OfflineAccess
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}