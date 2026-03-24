using CrmWebApi.DTOs.Phys;

namespace CrmWebApi.Services;

public interface IPhysService
{
    Task<IEnumerable<PhysResponse>> GetAllAsync();
    Task<PhysResponse> GetByIdAsync(int id);
    Task<PhysResponse> CreateAsync(CreatePhysRequest req);
    Task<PhysResponse> UpdateAsync(int id, UpdatePhysRequest req);
    Task DeleteAsync(int id);
    Task LinkOrgAsync(int physId, int orgId);
    Task UnlinkOrgAsync(int physId, int orgId);
}
