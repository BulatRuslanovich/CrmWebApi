using System.Linq.Expressions;
using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public IQueryable<Usr> QueryActive() =>
        db.Usrs.Include(u => u.UsrPolicies).ThenInclude(up => up.Policy);

    public Task<bool> ExistsAsync(Expression<Func<Usr, bool>> predicate) =>
        db.Usrs.AnyAsync(predicate);

    public async Task<Usr> AddAsync(Usr entity)
    {
        db.Usrs.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Usr entity)
    {
        db.Usrs.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies)
    {
        await db.UsrPolicies.AddRangeAsync(policies);
        await db.SaveChangesAsync();
    }

    public async Task LinkPolicyAsync(int userId, int policyId)
    {
        var exists = await db.UsrPolicies.AnyAsync(up => up.UsrId == userId && up.PolicyId == policyId);
        if (!exists)
        {
            db.UsrPolicies.Add(new UsrPolicy { UsrId = userId, PolicyId = policyId });
            await db.SaveChangesAsync();
        }
    }

    public async Task UnlinkPolicyAsync(int userId, int policyId)
    {
        var entry = await db.UsrPolicies.FirstOrDefaultAsync(up => up.UsrId == userId && up.PolicyId == policyId);
        if (entry is not null)
        {
            db.UsrPolicies.Remove(entry);
            await db.SaveChangesAsync();
        }
    }
}
