using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Org;
using CrmWebApi.DTOs.OrgType;

namespace CrmWebApi.Services;

public interface IOrgService
{
	public Task<PagedResponse<OrgResponse>> GetAllAsync(int page, int pageSize);
	public Task<OrgResponse> GetByIdAsync(int id);
	public Task<OrgResponse> CreateAsync(CreateOrgRequest req);
	public Task<OrgResponse> UpdateAsync(int id, UpdateOrgRequest req);
	public Task DeleteAsync(int id);

	public Task<IEnumerable<OrgTypeResponse>> GetAllTypesAsync();
}
