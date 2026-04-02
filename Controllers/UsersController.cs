using System.Security.Claims;
using CrmWebApi.DTOs.User;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/users")]
[Authorize]
public class UsersController(IUserService service) : ApiController
{
    [HttpGet("policies")]
    public async Task<IActionResult> GetAllPolicies() =>
        FromResult(await service.GetAllPoliciesAsync());

    [HttpGet("policies/{id:int}")]
    public async Task<IActionResult> GetPolicyById(int id) =>
        FromResult(await service.GetPolicyByIdAsync(id));

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        FromResult(await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id != currentUserId && !User.IsInRole("Admin"))
            return Forbid();
        return FromResult(await service.GetByIdAsync(id));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return FromResult(await service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await service.CreateAsync(request);
        return CreatedResult(result, nameof(GetById), new { id = result.Value?.UsrId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request) =>
        FromResult(await service.UpdateAsync(id, request));

    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id != currentUserId && !User.IsInRole("Admin"))
            return Forbid();
        return FromResult(await service.ChangePasswordAsync(id, request));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await service.DeleteAsync(id));

    [HttpPost("{id:int}/policies/{policyId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> LinkPolicy(int id, int policyId) =>
        FromResult(await service.LinkPolicyAsync(id, policyId));

    [HttpDelete("{id:int}/policies/{policyId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnlinkPolicy(int id, int policyId) =>
        FromResult(await service.UnlinkPolicyAsync(id, policyId));
}
