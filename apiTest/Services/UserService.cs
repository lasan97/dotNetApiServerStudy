using apiTest.Common.Data;
using apiTest.Common.Exceptions;
using apiTest.Domain.Entities;
using apiTest.Dtos.Users;
using apiTest.Services.Security;
using apiTest.Services.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace apiTest.Services;

public sealed class UserService(
    AppDbContext dbContext,
    IPasswordHashService passwordHashService)
{
    public async Task<List<UserResponse>> GetUsersAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .Select(user => new UserResponse(user.Id, user.Email, user.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserResponse> GetUserAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => new UserResponse(user.Id, user.Email, user.Name))
            .SingleOrDefaultAsync(cancellationToken);

        return user ?? throw new NotFoundException(nameof(User), id);
    }

    public async Task<UserResponse> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (await HasDuplicateEmailAsync(command.Email, null, cancellationToken))
        {
            throw CreateDuplicateEmailException(command.Email);
        }

        var user = new User(
            command.Email,
            command.Name,
            passwordHashService.Hash(command.Password));

        dbContext.Users.Add(user);
        await SaveChangesAsync(cancellationToken);

        return UserResponse.From(user);
    }

    public async Task<UserResponse> UpdateUserAsync(
        long id,
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        // DbContext가 이미 추적 중인 엔티티가 있는지 확인 후 있으면 해당 값을 반환 없다면 DB 조회
        var user = await dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        if (await HasDuplicateEmailAsync(command.Email, id, cancellationToken))
        {
            throw CreateDuplicateEmailException(command.Email);
        }

        user.Update(
            command.Email,
            command.Name,
            passwordHashService.Hash(command.Password));
        await SaveChangesAsync(cancellationToken);

        return UserResponse.From(user);
    }

    public async Task DeleteUserAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync(new object?[] { id }, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        dbContext.Users.Remove(user);
        await SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> HasDuplicateEmailAsync(
        string email,
        long? exceptUserId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(user =>
                    user.Email == email &&
                    (!exceptUserId.HasValue || user.Id != exceptUserId.Value),
                cancellationToken);
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            throw new ConflictException(
                "A user with the same email already exists.",
                "duplicate_user_email");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    private static ConflictException CreateDuplicateEmailException(string email)
    {
        return new ConflictException(
            $"A user with email '{email}' already exists.",
            "duplicate_user_email");
    }
}