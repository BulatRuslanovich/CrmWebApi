using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/physes")]
[Authorize]
public class PhysesController(IPhysService physService) : ApiController
{
    [HttpGet("specs")]
    public async Task<IActionResult> GetAllSpecs() =>
        FromResult(await physService.GetAllSpecsAsync());

    [HttpGet("specs/{id:int}")]
    public async Task<IActionResult> GetSpecById(int id) =>
        FromResult(await physService.GetSpecByIdAsync(id));

    [HttpPost("specs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSpec([FromBody] CreateSpecRequest req)
    {
        var result = await physService.CreateSpecAsync(req);
        return CreatedResult(result, nameof(GetSpecById), new { id = result.Value?.SpecId });
    }

    [HttpDelete("specs/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSpec(int id) =>
        FromResult(await physService.DeleteSpecAsync(id));

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        FromResult(await physService.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        FromResult(await physService.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePhysRequest req)
    {
        var result = await physService.CreateAsync(req);
        return CreatedResult(result, nameof(GetById), new { id = result.Value?.PhysId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePhysRequest req) =>
        FromResult(await physService.UpdateAsync(id, req));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await physService.DeleteAsync(id));

    [HttpPost("{physId:int}/orgs/{orgId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> LinkOrg(int physId, int orgId) =>
        FromResult(await physService.LinkOrgAsync(physId, orgId));

    [HttpDelete("{physId:int}/orgs/{orgId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnlinkOrg(int physId, int orgId) =>
        FromResult(await physService.UnlinkOrgAsync(physId, orgId));
}
