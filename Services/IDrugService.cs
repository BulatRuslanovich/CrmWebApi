using CrmWebApi.DTOs.Drug;

namespace CrmWebApi.Services;

public interface IDrugService
{
    Task<IEnumerable<DrugResponse>> GetAllAsync();
    Task<DrugResponse> GetByIdAsync(int id);
    Task<DrugResponse> CreateAsync(CreateDrugRequest req);
    Task<DrugResponse> UpdateAsync(int id, UpdateDrugRequest req);
    Task DeleteAsync(int id);
}
