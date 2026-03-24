using CrmWebApi.DTOs.Spec;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/specs")]
[Authorize]
public class SpecsController(ISpecService specService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<SpecResponse>> GetAll() =>
        await specService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<SpecResponse> GetById(int id) =>
        await specService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateSpecRequest req)
    {
        var result = await specService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.SpecId }, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await specService.DeleteAsync(id);
        return NoContent();
    }
}
