namespace apiTest.Services.Users.Dto;

public sealed record CreateUserCommand(
    string Email,
    string Name,
    string Password);