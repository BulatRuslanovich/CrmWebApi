using CrmWebApi.DTOs.Status;

namespace CrmWebApi.Services;

public interface IStatusService
{
    Task<IEnumerable<StatusResponse>> GetAllAsync();
    Task<StatusResponse> GetByIdAsync(int id);
    Task<StatusResponse> CreateAsync(CreateStatusRequest req);
    Task DeleteAsync(int id);
}
