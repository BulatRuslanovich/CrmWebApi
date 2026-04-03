using CrmWebApi.Common;
using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CrmWebApi.Services.Impl;

public class PhysService(IPhysRepository repo, AppDbContext db, IMemoryCache cache, ILogger<PhysService> logger) : IPhysService
{
    private CancellationTokenSource _cts = new();
    private const string AllSpecsKey = "specs";

    public async Task<Result<PagedResponse<PhysResponse>>> GetAllAsync(int page, int pageSize)
    {
        var query = repo.QueryActive();
        var total = await query.CountAsync();
        var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(MapToResponse).ToList();
        return new PagedResponse<PhysResponse>(items, page, pageSize, total);
    }

    public async Task<Result<PhysResponse>> GetByIdAsync(int id)
    {
        var phys = await repo.QueryActive().FirstOrDefaultAsync(p => p.PhysId == id);
        if (phys is null)
            return Error.NotFound($"Физическое лицо {id} не найдено");
        return MapToResponse(phys);
    }

    public async Task<Result<PhysResponse>> CreateAsync(CreatePhysRequest req)
    {
        var phys = new Phys
        {
            SpecId = req.SpecId,
            PhysFirstname = req.FirstName,
            PhysLastname = req.LastName,
            PhysMiddlename = req.MiddleName,
            PhysPhone = req.Phone,
            PhysEmail = req.Email,
            PhysPosition = req.Position
        };
        await repo.AddAsync(phys);
        logger.LogInformation("Contact created: id={PhysId}", phys.PhysId);
        return await GetByIdAsync(phys.PhysId);
    }

    public async Task<Result<PhysResponse>> UpdateAsync(int id, UpdatePhysRequest req)
    {
        var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id);
        if (phys is null)
            return Error.NotFound($"Физическое лицо {id} не найдено");

        phys.SpecId = req.SpecId ?? phys.SpecId;
        phys.PhysFirstname = req.FirstName ?? phys.PhysFirstname;
        phys.PhysLastname = req.LastName ?? phys.PhysLastname;
        phys.PhysMiddlename = req.MiddleName ?? phys.PhysMiddlename;
        phys.PhysPhone = req.Phone ?? phys.PhysPhone;
        phys.PhysEmail = req.Email ?? phys.PhysEmail;
        phys.PhysPosition = req.Position ?? phys.PhysPosition;

        await repo.UpdateAsync(phys);
        logger.LogInformation("Contact updated: id={PhysId}", id);
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id);
        if (phys is null)
            return Error.NotFound($"Физическое лицо {id} не найдено");

        phys.IsDeleted = true;
        await repo.UpdateAsync(phys);
        logger.LogInformation("Contact deleted: id={PhysId}", id);
        return Result.Success();
    }

    public async Task<Result> LinkOrgAsync(int physId, int orgId)
    {
        await repo.LinkOrgAsync(physId, orgId);
        return Result.Success();
    }

    public async Task<Result> UnlinkOrgAsync(int physId, int orgId)
    {
        await repo.UnlinkOrgAsync(physId, orgId);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<SpecResponse>>> GetAllSpecsAsync()
    {
        if (cache.TryGetValue(AllSpecsKey, out IEnumerable<SpecResponse>? cached))
            return Result<IEnumerable<SpecResponse>>.Success(cached!);

        var result = await db.Specs.Select(s => new SpecResponse(s.SpecId, s.SpecName)).ToListAsync();
        var options = new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(_cts.Token))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        cache.Set(AllSpecsKey, result, options);
        return Result<IEnumerable<SpecResponse>>.Success(result);
    }

    public async Task<Result<SpecResponse>> GetSpecByIdAsync(int id)
    {
        var spec = await db.Specs.FirstOrDefaultAsync(s => s.SpecId == id);
        if (spec is null)
            return Error.NotFound($"Специальность {id} не найдена");
        return new SpecResponse(spec.SpecId, spec.SpecName);
    }

    public async Task<Result<SpecResponse>> CreateSpecAsync(CreateSpecRequest req)
    {
        var spec = new Spec { SpecName = req.SpecName };
        db.Specs.Add(spec);
        await db.SaveChangesAsync();
        InvalidateCache();
        logger.LogInformation("Specialty created: {SpecName} (id={SpecId})", spec.SpecName, spec.SpecId);
        return new SpecResponse(spec.SpecId, spec.SpecName);
    }

    public async Task<Result> DeleteSpecAsync(int id)
    {
        var spec = await db.Specs.FirstOrDefaultAsync(s => s.SpecId == id);
        if (spec is null)
            return Error.NotFound($"Специальность {id} не найдена");

        spec.IsDeleted = true;
        await db.SaveChangesAsync();
        InvalidateCache();
        logger.LogInformation("Specialty deleted: id={SpecId}", id);
        return Result.Success();
    }

    private void InvalidateCache()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
    }

    private static PhysResponse MapToResponse(Phys p) => new(
        p.PhysId, p.SpecId, p.Spec?.SpecName,
        p.PhysFirstname, p.PhysLastname, p.PhysMiddlename,
        p.PhysPhone, p.PhysEmail, p.PhysPosition,
        [.. p.PhysOrgs.Where(po => po.Org is not null).Select(po => po.Org.OrgName)]
    );
}
