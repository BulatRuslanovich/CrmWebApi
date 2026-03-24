using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IUserRepository : IGenericRepository<Usr>
{
    IQueryable<Usr> QueryActive();
    Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies);
    Task AddPolicyAsync(int userId, int policyId);
    Task RemovePolicyAsync(int userId, int policyId);
}
