using CrmWebApi.Common;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Activ;

namespace CrmWebApi.Services;

public interface IActivService
{
    public Task<Result<PagedResponse<ActivResponse>>> GetAllAsync(int page, int pageSize, int? usrId = null);
    public Task<Result<ActivResponse>> GetByIdAsync(int id);
    public Task<Result<ActivResponse>> CreateAsync(int usrId, CreateActivRequest req);
    public Task<Result<ActivResponse>> UpdateAsync(int id, UpdateActivRequest req);
    public Task<Result> DeleteAsync(int id);
    public Task<Result> LinkDrugAsync(int activId, int drugId);
    public Task<Result> UnlinkDrugAsync(int activId, int drugId);
}
