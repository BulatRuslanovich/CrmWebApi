using CrmWebApi.DTOs.OrgType;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/org-types")]
[Authorize]
public class OrgTypesController(IOrgTypeService orgTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<OrgTypeResponse>> GetAll() =>
        await orgTypeService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<OrgTypeResponse> GetById(int id) =>
        await orgTypeService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOrgTypeRequest req)
    {
        var result = await orgTypeService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.OrgTypeId }, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await orgTypeService.DeleteAsync(id);
        return NoContent();
    }
}
