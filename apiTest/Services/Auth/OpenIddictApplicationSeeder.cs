using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Services.Auth;

public sealed class OpenIddictApplicationSeeder(
    IServiceProvider serviceProvider,
    IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var internalClientDescriptor = CreateInternalClientDescriptor(
            AuthConfiguration.GetInternalClientId(configuration),
            AuthConfiguration.GetInternalClientSecret(configuration));

        await EnsureInternalClientAsync(manager, internalClientDescriptor, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task EnsureInternalClientAsync(
        IOpenIddictApplicationManager manager,
        OpenIddictApplicationDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        var application = await manager.FindByClientIdAsync(descriptor.ClientId!, cancellationToken);
        if (application is null)
        {
            await manager.CreateAsync(descriptor, cancellationToken);
            return;
        }

        if (await manager.HasClientTypeAsync(application, ClientTypes.Confidential, cancellationToken) &&
            await manager.ValidateClientSecretAsync(application, descriptor.ClientSecret!, cancellationToken))
        {
            return;
        }

        await manager.UpdateAsync(application, descriptor, cancellationToken);
    }

    private static OpenIddictApplicationDescriptor CreateInternalClientDescriptor(
        string clientId,
        string clientSecret)
    {
        // password grant는 내부 first-party client에만 허용
        return new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            ClientType = ClientTypes.Confidential,
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
        };
    }
}
