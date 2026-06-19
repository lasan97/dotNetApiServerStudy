using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace apiTest.Domain.Entities;

// Data Annotation 적용
[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public long Id { get; private set; }
    
    [Required]
    [MaxLength(320)]
    public string Email { get; private set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; private set; } = null!;
    
    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; private set; } = null!;

    protected User()
    {
    }

    public User(string email, string name, string passwordHash)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
    }

    public void Update(string email, string name, string passwordHash)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
    }
}