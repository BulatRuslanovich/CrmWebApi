using CrmWebApi.DTOs.Org;

namespace CrmWebApi.Services;

public interface IOrgService
{
    Task<IEnumerable<OrgResponse>> GetAllAsync();
    Task<OrgResponse> GetByIdAsync(int id);
    Task<OrgResponse> CreateAsync(CreateOrgRequest req);
    Task<OrgResponse> UpdateAsync(int id, UpdateOrgRequest req);
    Task DeleteAsync(int id);
}
