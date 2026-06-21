using System.Globalization;
using System.Security.Claims;
using apiTest.Common.Data;
using apiTest.Domain.Entities;
using apiTest.Services.Auth.Dto;
using apiTest.Services.Security;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Services.Auth;

public sealed class AuthService(
    AppDbContext dbContext,
    IPasswordHashManager passwordHashManager)
{
    public async Task<ClaimsPrincipal?> CreatePasswordGrantPrincipalAsync(
        ValidateInternalUserCommand command,
        IEnumerable<string> scopes,
        CancellationToken cancellationToken)
    {
        var user = await ValidateInternalUserAsync(command, cancellationToken);

        return user is null
            ? null
            : CreatePrincipal(user, scopes);
    }

    private async Task<User?> ValidateInternalUserAsync(
        ValidateInternalUserCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            return null;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == command.Email, cancellationToken);

        if (user is null || !passwordHashManager.Verify(command.Password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }

    private static ClaimsPrincipal CreatePrincipal(User user, IEnumerable<string> scopes)
    {
        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            Claims.Name,
            Claims.Role);

        identity.AddClaim(new Claim(Claims.Subject, user.Id.ToString(CultureInfo.InvariantCulture)));
        identity.AddClaim(new Claim(Claims.Email, user.Email));
        identity.AddClaim(new Claim(Claims.Name, user.Name));

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(scopes);
        principal.SetResources(AuthConstants.ApiResource);

        // access token에 필요한 최소 사용자 식별 정보만 포함
        principal.SetDestinations(claim => claim.Type switch
        {
            Claims.Subject => [Destinations.AccessToken],
            Claims.Email => [Destinations.AccessToken],
            Claims.Name => [Destinations.AccessToken],
            _ => []
        });

        return principal;
    }
}