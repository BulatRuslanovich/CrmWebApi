using CrmWebApi.DTOs.User;

namespace CrmWebApi.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
    Task ChangePasswordAsync(int id, ChangePasswordRequest request);
    Task<UserResponse> LinkPolicyAsync(int userId, int policyId);
    Task<UserResponse> UnlinkPolicyAsync(int userId, int policyId);
}