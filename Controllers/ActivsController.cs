using System.Security.Claims;
using CrmWebApi.DTOs.Activ;
using CrmWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmWebApi.Controllers;

[Route("api/activs")]
[Authorize]
public class ActivsController(IActivService service) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        return FromResult(await service.GetAllAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), isAdmin ? null : usrId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (error, activ) = await AuthorizeOwnerAsync(id);
        if (error is not null) return error;
        return Ok(activ);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActivRequest req)
    {
        var usrId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(usrId, req);
        return CreatedResult(result, nameof(GetById), new { id = result.Value?.ActivId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateActivRequest req)
    {
        var (error, _) = await AuthorizeOwnerAsync(id);
        if (error is not null) return error;
        return FromResult(await service.UpdateAsync(id, req));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        FromResult(await service.DeleteAsync(id));

    [HttpPost("{activId:int}/drugs/{drugId:int}")]
    public async Task<IActionResult> LinkDrug(int activId, int drugId)
    {
        var (error, _) = await AuthorizeOwnerAsync(activId);
        if (error is not null) return error;
        return FromResult(await service.LinkDrugAsync(activId, drugId));
    }

    [HttpDelete("{activId:int}/drugs/{drugId:int}")]
    public async Task<IActionResult> UnlinkDrug(int activId, int drugId)
    {
        var (error, _) = await AuthorizeOwnerAsync(activId);
        if (error is not null) return error;
        return FromResult(await service.UnlinkDrugAsync(activId, drugId));
    }

    // Returns (error, activ): error is non-null if request should be rejected.
    private async Task<(IActionResult? error, ActivResponse? activ)> AuthorizeOwnerAsync(int activId)
    {
        var result = await service.GetByIdAsync(activId);
        if (!result.IsSuccess) return (MapError(result.Error!), null);

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (result.Value!.UsrId != currentUserId && !User.IsInRole("Admin"))
            return (Forbid(), null);

        return (null, result.Value);
    }
}
