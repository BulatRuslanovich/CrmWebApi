using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IPhysRepository : IGenericRepository<Phys>
{
    IQueryable<Phys> QueryActive();
    Task LinkOrgAsync(int physId, int orgId);
    Task UnlinkOrgAsync(int physId, int orgId);
}
