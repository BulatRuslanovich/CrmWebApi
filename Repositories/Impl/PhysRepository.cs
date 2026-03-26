using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class PhysRepository(AppDbContext db) : GenericRepository<Phys>(db), IPhysRepository
{
	public IQueryable<Phys> QueryActive() =>
		_dbSet
			.Include(p => p.Spec)
			.Include(p => p.PhysOrgs).ThenInclude(po => po.Org)
			.Where(p => !p.IsDeleted);

	public async Task LinkOrgAsync(int physId, int orgId)
	{
		_db.PhysOrgs.Add(new PhysOrg { PhysId = physId, OrgId = orgId });
		await _db.SaveChangesAsync();
	}

	public async Task UnlinkOrgAsync(int physId, int orgId)
	{
		var link = await _db.PhysOrgs.FirstOrDefaultAsync(po => po.PhysId == physId && po.OrgId == orgId)
			?? throw new KeyNotFoundException("Связь не найдена");
		_db.PhysOrgs.Remove(link);
		await _db.SaveChangesAsync();
	}
}
