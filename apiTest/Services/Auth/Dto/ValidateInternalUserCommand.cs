namespace apiTest.Services.Auth.Dto;

public sealed record ValidateInternalUserCommand(
    string? Email,
    string? Password);
