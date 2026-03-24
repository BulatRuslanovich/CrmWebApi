using CrmWebApi.DTOs.Policy;

namespace CrmWebApi.Services;

public interface IPolicyService
{
    Task<IEnumerable<PolicyResponse>> GetAllAsync();
    Task<PolicyResponse> GetByIdAsync(int id);
    Task<PolicyResponse> CreateAsync(CreatePolicyRequest req);
    Task DeleteAsync(int id);
}
