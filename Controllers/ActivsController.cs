using System.Security.Claims;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Activ;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/activs")]
[Authorize]
public class ActivsController(IActivService service) : ControllerBase
{

	// Получение всех активов. Если пользователь не админ, то возвращаем только его визиты
	[HttpGet]
	public async Task<PagedResponse<ActivResponse>> GetAll(
		[FromQuery] int page = 1, [FromQuery] int pageSize = 20)
	{
		var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		var isAdmin = User.IsInRole("Admin");
		return await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), isAdmin ? null : usrId);
	}


	[HttpGet("{id:int}")]
	public async Task<ActivResponse> GetById(int id) =>
		await service.GetByIdAsync(id);

	// Создание визита. Пользователь может создавать только свои визиты, поэтому берем id из токена
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateActivRequest req)
	{
		var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		var result = await service.CreateAsync(usrId, req);
		return CreatedAtAction(nameof(GetById), new { id = result.ActivId }, result);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateActivRequest req)
	{
		var activ = await service.GetByIdAsync(id);
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		if (activ.UsrId != currentUserId && !User.IsInRole("Admin"))
			return Forbid();
		return Ok(await service.UpdateAsync(id, req));
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id)
	{
		await service.DeleteAsync(id);
		return NoContent();
	}

	[HttpPost("{activId:int}/drugs/{drugId:int}")]
	public async Task<IActionResult> LinkDrug(int activId, int drugId)
	{
		var activ = await service.GetByIdAsync(activId);
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		if (activ.UsrId != currentUserId && !User.IsInRole("Admin"))
			return Forbid();
		await service.LinkDrugAsync(activId, drugId);
		return NoContent();
	}

	[HttpDelete("{activId:int}/drugs/{drugId:int}")]
	public async Task<IActionResult> UnlinkDrug(int activId, int drugId)
	{
		var activ = await service.GetByIdAsync(activId);
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
		if (activ.UsrId != currentUserId && !User.IsInRole("Admin"))
			return Forbid();
		await service.UnlinkDrugAsync(activId, drugId);
		return NoContent();
	}
}
