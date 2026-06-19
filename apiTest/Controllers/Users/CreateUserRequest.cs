using System.ComponentModel.DataAnnotations;

namespace apiTest.Dtos.Users;

public sealed class CreateUserRequest
{
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; init; } = string.Empty;

    [Required] [StringLength(100)] public string Name { get; init; } = string.Empty;

    [Required] [StringLength(200)] public string Password { get; init; } = string.Empty;
}