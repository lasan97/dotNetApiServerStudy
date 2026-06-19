namespace apiTest.Services.Users;

public sealed record CreateUserCommand(
    string Email,
    string Name,
    string Password);