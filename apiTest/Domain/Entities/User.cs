namespace apiTest.Domain.Entities;

public class User
{
    public const int EmailMaxLength = 320;
    public const int NameMaxLength = 100;
    public const int PasswordHashMaxLength = 500;

    public long Id { get; private set; }

    public string Email { get; private set; } = null!;

    public string Name { get; private set; } = null!;

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