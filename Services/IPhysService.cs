using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;

namespace CrmWebApi.Services;

public interface IPhysService
{
	public Task<PagedResponse<PhysResponse>> GetAllAsync(int page, int pageSize);
	public Task<PhysResponse> GetByIdAsync(int id);
	public Task<PhysResponse> CreateAsync(CreatePhysRequest req);
	public Task<PhysResponse> UpdateAsync(int id, UpdatePhysRequest req);
	public Task DeleteAsync(int id);
	public Task LinkOrgAsync(int physId, int orgId);
	public Task UnlinkOrgAsync(int physId, int orgId);
	public Task<IEnumerable<SpecResponse>> GetAllSpecsAsync();
	public Task<SpecResponse> GetSpecByIdAsync(int id);
	public Task<SpecResponse> CreateSpecAsync(CreateSpecRequest req);
	public Task DeleteSpecAsync(int id);
}
