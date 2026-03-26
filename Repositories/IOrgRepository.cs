using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IOrgRepository : IGenericRepository<Org>
{
	public IQueryable<Org> QueryActive();
	public IQueryable<OrgType> QueryOrgTypes();
}
