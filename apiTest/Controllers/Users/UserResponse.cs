using apiTest.Domain.Entities;

namespace apiTest.Dtos.Users;

public sealed record UserResponse(long Id, string Email, string Name)
{
    public static UserResponse From(User user)
    {
        return new UserResponse(user.Id, user.Email, user.Name);
    }
}