using apiTest.Services;
using apiTest.Services.Security;
using Microsoft.AspNetCore.Identity;

namespace apiTest.Extensions;

public static class ApplicationExtensions
{
    // Service DI
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<IPasswordHasher<PasswordHashSubject>, PasswordHasher<PasswordHashSubject>>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();

        return services;
    }
}
