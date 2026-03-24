using CrmWebApi.DTOs.OrgType;

namespace CrmWebApi.Services;

public interface IOrgTypeService
{
    Task<IEnumerable<OrgTypeResponse>> GetAllAsync();
    Task<OrgTypeResponse> GetByIdAsync(int id);
    Task<OrgTypeResponse> CreateAsync(CreateOrgTypeRequest req);
    Task DeleteAsync(int id);
}
