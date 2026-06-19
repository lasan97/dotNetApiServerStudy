using apiTest.Dtos.Users;
using apiTest.Services;
using apiTest.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace apiTest.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(UserService userService) : ControllerBase
{
    // 응답 타입에 List 대신 'IReadOnlyList'(불변리스트) 사용 가능
    [HttpGet]
    public async Task<List<UserResponse>> GetUsers(
        CancellationToken cancellationToken)
    {
        return await userService.GetUsersAsync(cancellationToken);
    }

    [HttpGet("{id:long}")]
    public async Task<UserResponse> GetUser(
        long id,
        CancellationToken cancellationToken)
    {
        return await userService.GetUserAsync(id, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Name,
            request.Password);
        var user = await userService.CreateUserAsync(command, cancellationToken);

        // location을 위한 첨부
        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            user);
    }

    [HttpPut("{id:long}")]
    public async Task<UserResponse> UpdateUser(
        long id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            request.Email,
            request.Name,
            request.Password);

        return await userService.UpdateUserAsync(id, command, cancellationToken);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(
        long id,
        CancellationToken cancellationToken)
    {
        await userService.DeleteUserAsync(id, cancellationToken);

        return NoContent();
    }
}