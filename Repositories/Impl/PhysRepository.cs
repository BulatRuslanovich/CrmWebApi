using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class PhysRepository(AppDbContext db) : IPhysRepository
{
    public IQueryable<Phys> QueryActive() =>
        db.Physes
            .Include(p => p.Spec)
            .Include(p => p.PhysOrgs).ThenInclude(po => po.Org);

    public IQueryable<Phys> Query() => db.Physes.AsQueryable();

    public async Task<Phys> AddAsync(Phys entity)
    {
        db.Physes.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Phys entity)
    {
        db.Physes.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task LinkOrgAsync(int physId, int orgId)
    {
        db.PhysOrgs.Add(new PhysOrg { PhysId = physId, OrgId = orgId });
        await db.SaveChangesAsync();
    }

    public async Task UnlinkOrgAsync(int physId, int orgId)
    {
        var link = await db.PhysOrgs.FirstOrDefaultAsync(po => po.PhysId == physId && po.OrgId == orgId)
            ?? throw new KeyNotFoundException("Связь не найдена");
        db.PhysOrgs.Remove(link);
        await db.SaveChangesAsync();
    }
}
