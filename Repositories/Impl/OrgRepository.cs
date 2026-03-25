using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class OrgRepository(AppDbContext db) : GenericRepository<Org>(db), IOrgRepository
{
    public IQueryable<Org> QueryActive() =>
        _dbSet
            .Include(o => o.OrgType)
            .Where(o => !o.IsDeleted);

    public IQueryable<OrgType> QueryOrgTypes() =>
        _db.OrgTypes.AsNoTracking();
}
