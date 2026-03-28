using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Drug;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/drugs")]
[Authorize]
public class DrugsController(IDrugService service) : ControllerBase
{
	[HttpGet]
	public async Task<PagedResponse<DrugResponse>> GetAll(
		[FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
		await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100));

	[HttpGet("{id:int}")]
	public async Task<DrugResponse> GetById(int id) =>
		await service.GetByIdAsync(id);

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create([FromBody] CreateDrugRequest req)
	{
		var result = await service.CreateAsync(req);
		return CreatedAtAction(nameof(GetById), new { id = result.DrugId }, result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<DrugResponse> Update(int id, [FromBody] UpdateDrugRequest req) =>
		await service.UpdateAsync(id, req);

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}
}
