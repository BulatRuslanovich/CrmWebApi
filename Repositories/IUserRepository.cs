using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IUserRepository : IGenericRepository<Usr>
{
	public IQueryable<Usr> QueryActive();
	public Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies);
	public Task LinkPolicyAsync(int userId, int policyId);
	public Task UnlinkPolicyAsync(int userId, int policyId);
}
