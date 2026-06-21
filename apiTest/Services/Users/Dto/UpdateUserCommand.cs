namespace apiTest.Services.Users.Dto;

public sealed record UpdateUserCommand(
    string Email,
    string Name,
    string Password);