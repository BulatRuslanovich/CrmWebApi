using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Drug;

namespace CrmWebApi.Services;

public interface IDrugService
{
	public Task<PagedResponse<DrugResponse>> GetAllAsync(int page, int pageSize);
	public Task<DrugResponse> GetByIdAsync(int id);
	public Task<DrugResponse> CreateAsync(CreateDrugRequest req);
	public Task<DrugResponse> UpdateAsync(int id, UpdateDrugRequest req);
	public Task DeleteAsync(int id);
}
