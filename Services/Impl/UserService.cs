using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.User;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Services.Impl;

public class UserService(IUserRepository repo) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync() =>
        await repo.QueryActive().Select(u => MapToResponse(u)).ToListAsync();

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
