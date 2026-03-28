using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/physes")]
[Authorize]
public class PhysesController(IPhysService physService) : ControllerBase
{
	[HttpGet("specs")]
	public async Task<IEnumerable<SpecResponse>> GetAllSpecs() =>
		await physService.GetAllSpecsAsync();

	[HttpGet("specs/{id:int}")]
	public async Task<SpecResponse> GetSpecById(int id) =>
		await physService.GetSpecByIdAsync(id);
	[HttpPost("specs")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> CreateSpec([FromBody] CreateSpecRequest req)
	{
		var result = await physService.CreateSpecAsync(req);
		return CreatedAtAction(nameof(GetSpecById), new { id = result.SpecId }, result);
	}

	[HttpDelete("specs/{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteSpec(int id)
	{
		await physService.DeleteSpecAsync(id);
		return NoContent();
	}

	[HttpGet]
	public async Task<PagedResponse<PhysResponse>> GetAll(
		[FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
		await physService.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100));

	[HttpGet("{id:int}")]
	public async Task<PhysResponse> GetById(int id) =>
		await physService.GetByIdAsync(id);

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create([FromBody] CreatePhysRequest req)
	{
		var result = await physService.CreateAsync(req);
		return CreatedAtAction(nameof(GetById), new { id = result.PhysId }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<PhysResponse> Update(int id, [FromBody] UpdatePhysRequest req) =>
		await physService.UpdateAsync(id, req);

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		await physService.DeleteAsync(id);
		return NoContent();
	}

	[HttpPost("{physId:int}/orgs/{orgId:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> LinkOrg(int physId, int orgId)
	{
		await physService.LinkOrgAsync(physId, orgId);
		return NoContent();
	}

	[HttpDelete("{physId:int}/orgs/{orgId:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UnlinkOrg(int physId, int orgId)
	{
		await physService.UnlinkOrgAsync(physId, orgId);
		return NoContent();
	}
}
