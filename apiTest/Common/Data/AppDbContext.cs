using apiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace apiTest.Common.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // OpenIddict가 사용하는 applications, authorizations, scopes, tokens 모델을 EF Core에 등록
        modelBuilder.UseOpenIddict();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}