using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class OrgRepository(AppDbContext db) : GenericRepository<Organization>(db), IOrgRepository
{
	public IQueryable<Organization> QueryActive() =>
		_dbSet
			.Include(o => o.OrgType)
			.Where(o => !o.IsDeleted);

	public IQueryable<OrgType> QueryOrgTypes() =>
		_db.OrgTypes.AsNoTracking();
}
