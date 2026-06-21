using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace apiTest.Services.Auth.Dto;

public sealed class TokenExchangeForm
{
    [FromForm(Name = "grant_type")]
    [DefaultValue("password")]
    public string? GrantType { get; init; }

    [FromForm(Name = "client_id")]
    [DefaultValue("internal-first-party")]
    public string? ClientId { get; init; }

    [FromForm(Name = "username")]
    [DefaultValue("user@example.com")]
    public string? Username { get; init; }

    [FromForm(Name = "password")]
    public string? Password { get; init; }

    [FromForm(Name = "scope")]
    [DefaultValue("api")]
    public string? Scope { get; init; }

    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; init; }
}