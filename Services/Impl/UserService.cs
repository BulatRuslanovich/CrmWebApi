using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Policy;
using CrmWebApi.DTOs.User;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CrmWebApi.Services.Impl;

public class UserService(IUserRepository repo, IGenericRepository<Policy> policyRepo, IMemoryCache cache) : IUserService
{
    public async Task<PagedResponse<UserResponse>> GetAllAsync(int page, int pageSize)
    {
        var query = repo.QueryActive();
        var total = await query.CountAsync();
        var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(MapToResponse).ToList();
        return new PagedResponse<UserResponse>(items, page, pageSize, total);
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id)
            ?? throw new KeyNotFoundException($"Пользователь {id} не найден");
        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest req)
    {
        if (await repo.ExistsAsync(u => u.UsrLogin == req.Login))
            throw new InvalidOperationException("Логин уже занят");

        var user = new Usr
        {
            UsrFirstname    = req.FirstName,
            UsrLastname     = req.LastName,
            UsrEmail        = req.Email,
            UsrPhone        = req.Phone,
            UsrLogin        = req.Login,
            UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };
        await repo.AddAsync(user);

        var policies = req.PolicyIds.Distinct()
            .Select(policyId => new UsrPolicy { UsrId = user.UsrId, PolicyId = policyId });
        await repo.AddPoliciesAsync(policies);

        return await GetByIdAsync(user.UsrId);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest req)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id)
            ?? throw new KeyNotFoundException($"Пользователь {id} не найден");

        user.UsrFirstname = req.FirstName ?? user.UsrFirstname;
        user.UsrLastname  = req.LastName  ?? user.UsrLastname;
        user.UsrEmail     = req.Email     ?? user.UsrEmail;
        user.UsrPhone     = req.Phone     ?? user.UsrPhone;

        await repo.UpdateAsync(user);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id)
            ?? throw new KeyNotFoundException($"Пользователь {id} не найден");
        user.IsDeleted = true;
        await repo.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(int id, ChangePasswordRequest req)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id)
            ?? throw new KeyNotFoundException($"Пользователь {id} не найден");

        if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, user.UsrPasswordHash))
            throw new UnauthorizedAccessException("Неверный текущий пароль");

        user.UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await repo.UpdateAsync(user);
    }

    public async Task<UserResponse> LinkPolicyAsync(int userId, int policyId)
    {
        await repo.LinkPolicyAsync(userId, policyId);
        return await GetByIdAsync(userId);
    }

    public async Task<UserResponse> UnlinkPolicyAsync(int userId, int policyId)
    {
        await repo.UnlinkPolicyAsync(userId, policyId);
        return await GetByIdAsync(userId);
    }

    private const string AllKey = "policies";

    public async Task<IEnumerable<PolicyResponse>> GetAllPoliciesAsync()
    {
        if (cache.TryGetValue(AllKey, out IEnumerable<PolicyResponse>? cached))
            return cached!;
        var result = (await policyRepo.FindAsync(p => !p.IsDeleted)).Select(MapToResponse).ToList();
        cache.Set(AllKey, result, TimeSpan.FromMinutes(10));
        return result;
    }

    public async Task<PolicyResponse> GetPolicyByIdAsync(int id)
    {
        var policy = await policyRepo.GetByIdAsync(id);
        if (policy is null || policy.IsDeleted)
            throw new KeyNotFoundException($"Политика {id} не найдена");
        return MapToResponse(policy);
    }

    private static PolicyResponse MapToResponse(Policy p) => new(p.PolicyId, p.PolicyName);

    private static UserResponse MapToResponse(Usr u) => new(
        u.UsrId,
        u.UsrFirstname,
        u.UsrLastname,
        u.UsrEmail,
        u.UsrPhone,
        u.UsrLogin,
        u.UsrPolicies.Where(p => !p.Policy.IsDeleted).Select(p => p.Policy.PolicyName).ToList()
    );
}
