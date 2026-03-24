using CrmWebApi.DTOs.Activ;

namespace CrmWebApi.Services;

public interface IActivService
{
    Task<IEnumerable<ActivResponse>> GetAllAsync(int? usrId = null);
    Task<ActivResponse> GetByIdAsync(int id);
    Task<ActivResponse> CreateAsync(int usrId, CreateActivRequest req);
    Task<ActivResponse> UpdateAsync(int id, UpdateActivRequest req);
    Task DeleteAsync(int id);
    Task AddDrugAsync(int activId, int drugId);
    Task RemoveDrugAsync(int activId, int drugId);
}
