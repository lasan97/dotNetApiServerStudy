using apiTest.Services.Auth;
using apiTest.Services.Auth.Dto;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace apiTest.Controllers;

[ApiController]
public sealed class ConnectController(AuthService authService) : ControllerBase
{
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Exchange(CancellationToken cancellationToken)
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("OpenIddict token request is not available.");

        if (request.IsPasswordGrantType())
        {
            var principal = await authService.CreatePasswordGrantPrincipalAsync(
                new ValidateInternalUserCommand(
                    request.Username,
                    request.Password),
                request.GetScopes(),
                cancellationToken);

            if (principal is null)
            {
                return InvalidGrant("The username/password couple is invalid.");
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal is null)
            {
                return InvalidGrant("The refresh token is invalid.");
            }

            // refresh token 요청에서는 기존 principal을 다시 서명해 새 access token을 발급
            return SignIn(result.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return InvalidGrant("The specified grant type is not supported.");
    }

    private ForbidResult InvalidGrant(string description)
    {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
            }));
    }
}
