using CrmWebApi.DTOs.Status;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/statuses")]
[Authorize]
public class StatusesController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<StatusResponse>> GetAll() =>
        await statusService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<StatusResponse> GetById(int id) =>
        await statusService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateStatusRequest req)
    {
        var result = await statusService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.StatusId }, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await statusService.DeleteAsync(id);
        return NoContent();
    }
}
