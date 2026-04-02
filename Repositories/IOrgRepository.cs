using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IOrgRepository
{
    public IQueryable<Organization> QueryActive();
    public IQueryable<Organization> Query();
    public IQueryable<OrgType> QueryOrgTypes();
    public Task<Organization> AddAsync(Organization entity);
    public Task UpdateAsync(Organization entity);
}
