using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IPhysRepository : IGenericRepository<Phys>
{
	public IQueryable<Phys> QueryActive();
	public Task LinkOrgAsync(int physId, int orgId);
	public Task UnlinkOrgAsync(int physId, int orgId);
}
