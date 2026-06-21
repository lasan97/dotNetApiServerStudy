using apiTest.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace apiTest.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            // OpenIddict EF Core store가 AppDbContext를 토큰 저장소로 사용할 수 있게 연결
            options.UseOpenIddict();
        });

        return services;
    }
}