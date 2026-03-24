using CrmWebApi.DTOs.User;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Director")]
    public async Task<IEnumerable<UserResponse>> GetAll() =>
        await userService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<UserResponse> GetById(int id) =>
        await userService.GetByIdAsync(id);

    [HttpGet("me")]
    public async Task<UserResponse> GetMe()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await userService.GetByIdAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.UsrId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<UserResponse> Update(int id, [FromBody] UpdateUserRequest request) =>
        await userService.UpdateAsync(id, request);

    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        await userService.ChangePasswordAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/policies/{policyId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<UserResponse> AddPolicy(int id, int policyId) =>
        await userService.AddPolicyAsync(id, policyId);

    [HttpDelete("{id:int}/policies/{policyId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<UserResponse> RemovePolicy(int id, int policyId) =>
        await userService.RemovePolicyAsync(id, policyId);
}
