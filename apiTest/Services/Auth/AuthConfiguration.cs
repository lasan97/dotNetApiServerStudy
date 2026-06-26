namespace apiTest.Services.Auth;

public static class AuthConfiguration
{
    private const string InternalClientIdKey = "Auth:InternalClientId";
    private const string InternalClientSecretKey = "Auth:InternalClientSecret";

    public static string GetInternalClientId(IConfiguration configuration)
    {
        return GetRequiredValue(configuration, InternalClientIdKey);
    }

    public static string GetInternalClientSecret(IConfiguration configuration)
    {
        return GetRequiredValue(configuration, InternalClientSecretKey);
    }

    private static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} must be configured.");
        }

        return value;
    }
}
