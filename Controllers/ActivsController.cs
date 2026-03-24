using System.Security.Claims;
using CrmWebApi.DTOs.Activ;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/activs")]
[Authorize]
public class ActivsController(IActivService activService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ActivResponse>> GetAll()
    {
        var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        return await activService.GetAllAsync(isAdmin ? null : usrId);
    }

    [HttpGet("{id:int}")]
    public async Task<ActivResponse> GetById(int id) =>
        await activService.GetByIdAsync(id);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActivRequest req)
    {
        var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await activService.CreateAsync(usrId, req);
        return CreatedAtAction(nameof(GetById), new { id = result.ActivId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActivResponse> Update(int id, [FromBody] UpdateActivRequest req) =>
        await activService.UpdateAsync(id, req);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await activService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{activId:int}/drugs/{drugId:int}")]
    public async Task<IActionResult> AddDrug(int activId, int drugId)
    {
        await activService.AddDrugAsync(activId, drugId);
        return NoContent();
    }

    [HttpDelete("{activId:int}/drugs/{drugId:int}")]
    public async Task<IActionResult> RemoveDrug(int activId, int drugId)
    {
        await activService.RemoveDrugAsync(activId, drugId);
        return NoContent();
    }
}
