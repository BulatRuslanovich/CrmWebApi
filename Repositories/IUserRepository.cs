using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IUserRepository
{
    public IQueryable<Usr> QueryActive();
    public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Usr, bool>> predicate);
    public Task<Usr> AddAsync(Usr entity);
    public Task<Usr> AddWithPoliciesAsync(Usr entity, IEnumerable<int> policyIds);
    public Task UpdateAsync(Usr entity);
    public Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies);
    public Task LinkPolicyAsync(int userId, int policyId);
    public Task UnlinkPolicyAsync(int userId, int policyId);
}
