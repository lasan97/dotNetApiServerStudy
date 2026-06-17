using apiTest.Services;

namespace apiTest.Extensions;

public static class ApplicationExtensions
{
    // Service DI
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<FirstService>();

        return services;
    }
}