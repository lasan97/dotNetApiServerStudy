using apiTest.Common.Data;
using apiTest.Domain.Entities;

namespace apiTest.Services;

public class FirstService(AppDbContext dbContext)
{
    public string getMessage()
    {
        return dbContext.Users.Count().ToString();
    }
}