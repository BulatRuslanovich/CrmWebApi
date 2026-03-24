using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Policy;
using CrmWebApi.Repositories;

namespace CrmWebApi.Services.Impl;

public class PolicyService(IGenericRepository<Policy> repo) : IPolicyService
{
    public async Task<IEnumerable<PolicyResponse>> GetAllAsync() =>
        (await repo.FindAsync(p => !p.IsDeleted)).Select(MapToResponse);

    public async Task<PolicyResponse> GetByIdAsync(int id)
    {
        var policy = await repo.GetByIdAsync(id);
        if (policy is null || policy.IsDeleted)
            throw new KeyNotFoundException($"Политика {id} не найдена");
        return MapToResponse(policy);
    }

    public async Task<PolicyResponse> CreateAsync(CreatePolicyRequest req)
    {
        var policy = new Policy
        {
            PolicyName = req.PolicyName,
        };
        await repo.AddAsync(policy);
        return MapToResponse(policy);
    }

    public async Task DeleteAsync(int id)
    {
        var policy = await repo.GetByIdAsync(id);
        if (policy is null || policy.IsDeleted)
            throw new KeyNotFoundException($"Политика {id} не найдена");
        policy.IsDeleted = true;
        await repo.UpdateAsync(policy);
    }

    private static PolicyResponse MapToResponse(Policy p) => new(p.PolicyId, p.PolicyName);
}
