using CrmWebApi.DTOs.Policy;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[ApiController]
[Route("api/policies")]
[Authorize]
public class PoliciesController(IPolicyService policyService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<PolicyResponse>> GetAll() =>
        await policyService.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<PolicyResponse> GetById(int id) =>
        await policyService.GetByIdAsync(id);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePolicyRequest req)
    {
        var result = await policyService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = result.PolicyId }, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await policyService.DeleteAsync(id);
        return NoContent();
    }
}
