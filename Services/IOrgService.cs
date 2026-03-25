using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Org;
using CrmWebApi.DTOs.OrgType;

namespace CrmWebApi.Services;

public interface IOrgService
{
    Task<PagedResponse<OrgResponse>> GetAllAsync(int page, int pageSize);
    Task<OrgResponse> GetByIdAsync(int id);
    Task<OrgResponse> CreateAsync(CreateOrgRequest req);
    Task<OrgResponse> UpdateAsync(int id, UpdateOrgRequest req);
    Task DeleteAsync(int id);

    Task<IEnumerable<OrgTypeResponse>> GetAllTypesAsync();
}
