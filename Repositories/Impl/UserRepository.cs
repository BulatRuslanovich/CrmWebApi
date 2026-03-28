using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class UserRepository(AppDbContext db) : GenericRepository<Usr>(db), IUserRepository
{
	public IQueryable<Usr> QueryActive() =>
		_dbSet
			.Include(u => u.UsrPolicies).ThenInclude(up => up.Policy);

	public async Task AddPoliciesAsync(IEnumerable<UsrPolicy> policies)
	{
		await _db.UsrPolicies.AddRangeAsync(policies);
		await _db.SaveChangesAsync();
	}

	public async Task LinkPolicyAsync(int userId, int policyId)
	{
		var exists = await _db.UsrPolicies
			.AnyAsync(up => up.UsrId == userId && up.PolicyId == policyId);
		if (!exists)
		{
			await _db.UsrPolicies.AddAsync(new UsrPolicy { UsrId = userId, PolicyId = policyId });
			await _db.SaveChangesAsync();
		}
	}

	public async Task UnlinkPolicyAsync(int userId, int policyId)
	{
		var entry = await _db.UsrPolicies
			.FirstOrDefaultAsync(up => up.UsrId == userId && up.PolicyId == policyId);
		if (entry is not null)
		{
			_db.UsrPolicies.Remove(entry);
			await _db.SaveChangesAsync();
		}
	}
}
