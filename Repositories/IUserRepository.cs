using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IUserRepository : IGenericRepository<Usr>
{
    IQueryable<Usr> QueryActive();
    Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies);
    Task LinkPolicyAsync(int userId, int policyId);
    Task UnlinkPolicyAsync(int userId, int policyId);
}
