namespace apiTest.Services.Users;

public sealed record UpdateUserCommand(
    string Email,
    string Name,
    string Password);