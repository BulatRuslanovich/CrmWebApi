using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Org;
using CrmWebApi.DTOs.OrgType;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/orgs")]
[Authorize]
public class OrgsController(IOrgService service) : ControllerBase
{
	[HttpGet("types")]
	public async Task<IEnumerable<OrgTypeResponse>> GetAllTypes() =>
		await service.GetAllTypesAsync();

	[HttpGet]
	public async Task<PagedResponse<OrgResponse>> GetAll(
		[FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
		await service.GetAllAsync(page, Math.Min(pageSize, 100));

	[HttpGet("{id:int}")]
	public async Task<OrgResponse> GetById(int id) =>
		await service.GetByIdAsync(id);


	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create([FromBody] CreateOrgRequest req)
	{
		var result = await service.CreateAsync(req);
		return CreatedAtAction(nameof(GetById), new { id = result.OrgId }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<OrgResponse> Update(int id, [FromBody] UpdateOrgRequest req) =>
		await service.UpdateAsync(id, req);

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}
}
