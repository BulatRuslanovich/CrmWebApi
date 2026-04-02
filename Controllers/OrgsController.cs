using CrmWebApi.DTOs.Org;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/orgs")]
[Authorize]
public class OrgsController(IOrgService service) : ApiController
{
    [HttpGet("types")]
    public async Task<IActionResult> GetAllTypes() =>
        FromResult(await service.GetAllTypesAsync());

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        FromResult(await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        FromResult(await service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOrgRequest req)
    {
        var result = await service.CreateAsync(req);
        return CreatedResult(result, nameof(GetById), new { id = result.Value?.OrgId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrgRequest req) =>
        FromResult(await service.UpdateAsync(id, req));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await service.DeleteAsync(id));
}
