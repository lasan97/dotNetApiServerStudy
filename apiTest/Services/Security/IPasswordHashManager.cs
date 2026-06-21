namespace apiTest.Services.Security;

public interface IPasswordHashManager
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
