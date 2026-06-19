using apiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace apiTest.Common.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

}