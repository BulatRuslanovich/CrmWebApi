using CrmWebApi.DTOs.Drug;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/drugs")]
[Authorize]
public class DrugsController(IDrugService drugService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<DrugResponse>> GetAll() =>
        await drugService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<DrugResponse> GetById(int id) =>
        await drugService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDrugRequest req)
    {
        var result = await drugService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.DrugId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<DrugResponse> Update(int id, [FromBody] UpdateDrugRequest req) =>
        await drugService.UpdateAsync(id, req);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await drugService.DeleteAsync(id);
        return NoContent();
    }
}
