using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Activ;

namespace CrmWebApi.Services;

public interface IActivService
{
	public Task<PagedResponse<ActivResponse>> GetAllAsync(int page, int pageSize, int? usrId = null);
	public Task<ActivResponse> GetByIdAsync(int id);
	public Task<ActivResponse> CreateAsync(int usrId, CreateActivRequest req);
	public Task<ActivResponse> UpdateAsync(int id, UpdateActivRequest req);
	public Task DeleteAsync(int id);
	public Task LinkDrugAsync(int activId, int drugId);
	public Task UnlinkDrugAsync(int activId, int drugId);
}
