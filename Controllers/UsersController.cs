using System.Security.Claims;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Policy;
using CrmWebApi.DTOs.User;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService service) : ControllerBase
{
	[HttpGet("policies")]
	public async Task<IEnumerable<PolicyResponse>> GetAllPolicies() =>
		await service.GetAllPoliciesAsync();

	[HttpGet("policies/{id:int}")]
	public async Task<PolicyResponse> GetPolicyById(int id) =>
		await service.GetPolicyByIdAsync(id);

	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<PagedResponse<UserResponse>> GetAll(
		[FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
		await service.GetAllAsync(page, Math.Min(pageSize, 100));

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		if (id != currentUserId && !User.IsInRole("Admin"))
			return Forbid();
		return Ok(await service.GetByIdAsync(id));
	}

	[HttpGet("me")]
	public async Task<UserResponse> GetMe()
	{
		var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		return await service.GetByIdAsync(id);
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
	{
		var result = await service.CreateAsync(request);
		return CreatedAtAction(nameof(GetById), new { id = result.UsrId }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<UserResponse> Update(int id, [FromBody] UpdateUserRequest request) =>
		await service.UpdateAsync(id, request);

	[HttpPatch("{id:int}/password")]
	public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
	{
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		if (id != currentUserId && !User.IsInRole("Admin"))
			return Forbid();
		await service.ChangePasswordAsync(id, request);
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}

	[HttpPost("{id:int}/policies/{policyId:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<UserResponse> LinkPolicy(int id, int policyId) =>
		await service.LinkPolicyAsync(id, policyId);

	[HttpDelete("{id:int}/policies/{policyId:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<UserResponse> UnlinkPolicy(int id, int policyId) =>
		await service.UnlinkPolicyAsync(id, policyId);
}
