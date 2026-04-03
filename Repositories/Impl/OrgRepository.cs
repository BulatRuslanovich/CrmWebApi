using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class OrgRepository(AppDbContext db) : IOrgRepository
{
    public IQueryable<Organization> QueryActive() =>
        db.Orgs.Include(o => o.OrgType).AsNoTracking();

    public IQueryable<Organization> Query() => db.Orgs.AsQueryable();

    public IQueryable<OrgType> QueryOrgTypes() => db.OrgTypes.AsNoTracking();

    public async Task<Organization> AddAsync(Organization entity)
    {
        db.Orgs.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Organization entity)
    {
        db.Orgs.Update(entity);
        await db.SaveChangesAsync();
    }
}
