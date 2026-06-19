namespace apiTest.Domain.Entities;

public class User
{
    public long Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

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