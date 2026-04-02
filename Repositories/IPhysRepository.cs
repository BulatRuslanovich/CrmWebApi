using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IPhysRepository
{
    public IQueryable<Phys> QueryActive();
    public IQueryable<Phys> Query();
    public Task<Phys> AddAsync(Phys entity);
    public Task UpdateAsync(Phys entity);
    public Task LinkOrgAsync(int physId, int orgId);
    public Task UnlinkOrgAsync(int physId, int orgId);
}
