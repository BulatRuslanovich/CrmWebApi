using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;

namespace CrmWebApi.Services;

public interface IPhysService
{
    Task<PagedResponse<PhysResponse>> GetAllAsync(int page, int pageSize);
    Task<PhysResponse> GetByIdAsync(int id);
    Task<PhysResponse> CreateAsync(CreatePhysRequest req);
    Task<PhysResponse> UpdateAsync(int id, UpdatePhysRequest req);
    Task DeleteAsync(int id);
    Task LinkOrgAsync(int physId, int orgId);
    Task UnlinkOrgAsync(int physId, int orgId);
    Task<IEnumerable<SpecResponse>> GetAllSpecsAsync();
    Task<SpecResponse> GetSpecByIdAsync(int id);
    Task<SpecResponse> CreateSpecAsync(CreateSpecRequest req);
    Task DeleteSpecAsync(int id);
}
