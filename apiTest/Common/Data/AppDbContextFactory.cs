using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace apiTest.Common.Data;

// EF Core CLI가 마이그레이션 생성/적용 같은 작업에서 AppDbContext를 만들 때 사용
// 일반 서버 실행 시에는 Extensions/DatabaseExtensions.cs의 DI 등록이 AppDbContext 생성을 담당
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            // 디자인타임 마이그레이션 생성에서도 OpenIddict 모델을 동일하게 포함
            .UseOpenIddict()
            .Options;

        return new AppDbContext(options);
    }
}