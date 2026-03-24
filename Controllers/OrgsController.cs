using CrmWebApi.DTOs.Org;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/orgs")]
[Authorize]
public class OrgsController(IOrgService orgService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<OrgResponse>> GetAll() =>
        await orgService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<OrgResponse> GetById(int id) =>
        await orgService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOrgRequest req)
    {
        var result = await orgService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.OrgId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<OrgResponse> Update(int id, [FromBody] UpdateOrgRequest req) =>
        await orgService.UpdateAsync(id, req);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await orgService.DeleteAsync(id);
        return NoContent();
    }
}
