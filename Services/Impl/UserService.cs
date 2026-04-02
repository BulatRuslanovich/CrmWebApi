using CrmWebApi.Common;
using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Policy;
using CrmWebApi.DTOs.User;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CrmWebApi.Services.Impl;

public class UserService(IUserRepository repo, AppDbContext db, IMemoryCache cache, ILogger<UserService> logger) : IUserService
{
    public async Task<Result<PagedResponse<UserResponse>>> GetAllAsync(int page, int pageSize)
    {
        var query = repo.QueryActive();
        var total = await query.CountAsync();
        var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(MapToResponse).ToList();
        return new PagedResponse<UserResponse>(items, page, pageSize, total);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(int id)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id);
        if (user is null)
            return Error.NotFound($"Пользователь {id} не найден");
        return MapToResponse(user);
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest req)
    {
        if (await repo.ExistsAsync(u => u.UsrLogin == req.Login))
            return Error.Conflict("Логин уже занят");

        var user = new Usr
        {
            UsrFirstname = req.FirstName,
            UsrLastname = req.LastName,
            UsrEmail = req.Email,
            UsrPhone = req.Phone,
            UsrLogin = req.Login,
            UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };
        await repo.AddAsync(user);

        var policies = req.PolicyIds.Distinct()
            .Select(policyId => new UsrPolicy { UsrId = user.UsrId, PolicyId = policyId });
        await repo.AddPoliciesAsync(policies);

        logger.LogInformation("Пользователь создан: {Login} (id={UsrId})", user.UsrLogin, user.UsrId);
        return await GetByIdAsync(user.UsrId);
    }

    public async Task<Result<UserResponse>> UpdateAsync(int id, UpdateUserRequest req)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id);
        if (user is null)
            return Error.NotFound($"Пользователь {id} не найден");

        user.UsrFirstname = req.FirstName ?? user.UsrFirstname;
        user.UsrLastname = req.LastName ?? user.UsrLastname;
        user.UsrEmail = req.Email ?? user.UsrEmail;
        user.UsrPhone = req.Phone ?? user.UsrPhone;

        await repo.UpdateAsync(user);
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id);
        if (user is null)
            return Error.NotFound($"Пользователь {id} не найден");

        user.IsDeleted = true;
        await repo.UpdateAsync(user);
        logger.LogInformation("Пользователь удалён: id={UsrId}", id);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest req)
    {
        var user = await repo.QueryActive().FirstOrDefaultAsync(u => u.UsrId == id);
        if (user is null)
            return Error.NotFound($"Пользователь {id} не найден");

        if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, user.UsrPasswordHash))
            return Error.Unauthorized("Неверный текущий пароль");

        user.UsrPasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await repo.UpdateAsync(user);
        logger.LogInformation("Пароль изменён: id={UsrId}", id);
        return Result.Success();
    }

    public async Task<Result<UserResponse>> LinkPolicyAsync(int userId, int policyId)
    {
        await repo.LinkPolicyAsync(userId, policyId);
        return await GetByIdAsync(userId);
    }

    public async Task<Result<UserResponse>> UnlinkPolicyAsync(int userId, int policyId)
    {
        await repo.UnlinkPolicyAsync(userId, policyId);
        return await GetByIdAsync(userId);
    }

    private const string AllPoliciesKey = "policies";

    public async Task<Result<IEnumerable<PolicyResponse>>> GetAllPoliciesAsync()
    {
        if (cache.TryGetValue(AllPoliciesKey, out IEnumerable<PolicyResponse>? cached))
            return Result<IEnumerable<PolicyResponse>>.Success(cached!);

        var result = await db.Policies.Select(p => new PolicyResponse(p.PolicyId, p.PolicyName)).ToListAsync();
        cache.Set(AllPoliciesKey, result, TimeSpan.FromMinutes(10));
        return Result<IEnumerable<PolicyResponse>>.Success(result);
    }

    public async Task<Result<PolicyResponse>> GetPolicyByIdAsync(int id)
    {
        var policy = await db.Policies.FirstOrDefaultAsync(p => p.PolicyId == id);
        if (policy is null)
            return Error.NotFound($"Политика {id} не найдена");
        return new PolicyResponse(policy.PolicyId, policy.PolicyName);
    }

    private static UserResponse MapToResponse(Usr u) => new(
        u.UsrId,
        u.UsrFirstname,
        u.UsrLastname,
        u.UsrEmail,
        u.UsrPhone,
        u.UsrLogin,
        [.. u.UsrPolicies.Where(p => p.Policy is not null).Select(p => p.Policy.PolicyName)]
    );
}
