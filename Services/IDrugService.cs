using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Drug;

namespace CrmWebApi.Services;

public interface IDrugService
{
    Task<PagedResponse<DrugResponse>> GetAllAsync(int page, int pageSize);
    Task<DrugResponse> GetByIdAsync(int id);
    Task<DrugResponse> CreateAsync(CreateDrugRequest req);
    Task<DrugResponse> UpdateAsync(int id, UpdateDrugRequest req);
    Task DeleteAsync(int id);
}
