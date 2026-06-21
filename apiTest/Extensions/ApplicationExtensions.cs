using apiTest.Services.Auth;
using apiTest.Services.Security;
using apiTest.Services.Users;
using Microsoft.AspNetCore.Identity;

namespace apiTest.Extensions;

public static class ApplicationExtensions
{
    // Service DI
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Service
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        
        // Helper
        services.AddScoped<IPasswordHasher<PasswordHashSubject>, PasswordHasher<PasswordHashSubject>>();
        services.AddScoped<IPasswordHashManager, PasswordHashManager>();

        return services;
    }
}
