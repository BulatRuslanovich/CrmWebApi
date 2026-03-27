using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IOrgRepository : IGenericRepository<Organization>
{
	public IQueryable<Organization> QueryActive();
	public IQueryable<OrgType> QueryOrgTypes();
}
