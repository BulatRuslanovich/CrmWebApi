using CrmWebApi.DTOs.Drug;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/drugs")]
[Authorize]
public class DrugsController(IDrugService service) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        FromResult(await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        FromResult(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDrugRequest req)
    {
        var result = await service.CreateAsync(req);
        return CreatedResult(result, nameof(GetById), new { id = result.Value?.DrugId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDrugRequest req) =>
        FromResult(await service.UpdateAsync(id, req));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await service.DeleteAsync(id));
}
