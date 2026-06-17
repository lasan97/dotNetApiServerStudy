namespace apiTest.Domain.Entities;

public class User
{
    public long Id { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }

    protected User()
    {
    }

    public User(string email, string name, string password)
    {
        Email = email;
        Name = name;
        Password = password;
    }
}