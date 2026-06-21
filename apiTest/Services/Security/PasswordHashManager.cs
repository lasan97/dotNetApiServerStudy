using Microsoft.AspNetCore.Identity;

namespace apiTest.Services.Security;

public sealed class PasswordHashSubject;

public sealed class PasswordHashManager(
    IPasswordHasher<PasswordHashSubject> passwordHasher) : IPasswordHashManager
{
    private static readonly PasswordHashSubject Subject = new();

    public string Hash(string password)
    {
        return passwordHasher.HashPassword(Subject, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        try
        {
            var result = passwordHasher.VerifyHashedPassword(
                Subject,
                passwordHash,
                password);

            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
