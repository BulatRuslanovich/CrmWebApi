using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Policy;
using CrmWebApi.DTOs.User;

namespace CrmWebApi.Services;

public interface IUserService
{
	public Task<PagedResponse<UserResponse>> GetAllAsync(int page, int pageSize);
	public Task<UserResponse> GetByIdAsync(int id);
	public Task<UserResponse> CreateAsync(CreateUserRequest request);
	public Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
	public Task DeleteAsync(int id);
	public Task ChangePasswordAsync(int id, ChangePasswordRequest request);
	public Task<UserResponse> LinkPolicyAsync(int userId, int policyId);
	public Task<UserResponse> UnlinkPolicyAsync(int userId, int policyId);
	public Task<IEnumerable<PolicyResponse>> GetAllPoliciesAsync();
	public Task<PolicyResponse> GetPolicyByIdAsync(int id);
}
