using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Services.Impl;

public class PhysService(IPhysRepository repo) : IPhysService
{
    public async Task<IEnumerable<PhysResponse>> GetAllAsync() =>
        await repo.QueryActive().Select(p => MapToResponse(p)).ToListAsync();

    public async Task<PhysResponse> GetByIdAsync(int id)
    {
        var phys = await repo.QueryActive().FirstOrDefaultAsync(p => p.PhysId == id)
            ?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");
        return MapToResponse(phys);
    }

    public async Task<PhysResponse> CreateAsync(CreatePhysRequest req)
    {
        var phys = new Phys
        {
            SpecId         = req.SpecId,
            PhysFirstname  = req.FirstName,
            PhysLastname   = req.LastName,
            PhysMiddlename = req.MiddleName,
            PhysPhone      = req.Phone,
            PhysEmail      = req.Email,
            PhysPosition   = req.Position
        };
        await repo.AddAsync(phys);
        return await GetByIdAsync(phys.PhysId);
    }

    public async Task<PhysResponse> UpdateAsync(int id, UpdatePhysRequest req)
    {
        var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");

        phys.SpecId         = req.SpecId        ?? phys.SpecId;
        phys.PhysFirstname  = req.FirstName      ?? phys.PhysFirstname;
        phys.PhysLastname   = req.LastName       ?? phys.PhysLastname;
        phys.PhysMiddlename = req.MiddleName     ?? phys.PhysMiddlename;
        phys.PhysPhone      = req.Phone          ?? phys.PhysPhone;
        phys.PhysEmail      = req.Email          ?? phys.PhysEmail;
        phys.PhysPosition   = req.Position       ?? phys.PhysPosition;

        await repo.UpdateAsync(phys);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");
        phys.IsDeleted = true;
        await repo.UpdateAsync(phys);
    }

    public async Task LinkOrgAsync(int physId, int orgId) =>
        await repo.LinkOrgAsync(physId, orgId);

    public async Task UnlinkOrgAsync(int physId, int orgId) =>
        await repo.UnlinkOrgAsync(physId, orgId);

    private static PhysResponse MapToResponse(Phys p) => new(
        p.PhysId, p.SpecId, p.Spec?.SpecName,
        p.PhysFirstname, p.PhysLastname, p.PhysMiddlename,
        p.PhysPhone, p.PhysEmail, p.PhysPosition,
        [.. p.PhysOrgs.Where(po => !po.Org.IsDeleted).Select(po => po.Org.OrgName)]
    );
}
