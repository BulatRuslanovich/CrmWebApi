using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Status;
using CrmWebApi.Repositories;

namespace CrmWebApi.Services.Impl;

public class StatusService(IGenericRepository<Status> repo) : IStatusService
{
    public async Task<IEnumerable<StatusResponse>> GetAllAsync() =>
        (await repo.FindAsync(s => !s.IsDeleted)).Select(MapToResponse);

    public async Task<StatusResponse> GetByIdAsync(int id)
    {
        var status = await repo.GetByIdAsync(id);
        if (status is null || status.IsDeleted)
            throw new KeyNotFoundException($"Статус {id} не найден");
        return MapToResponse(status);
    }

    public async Task<StatusResponse> CreateAsync(CreateStatusRequest req)
    {
        var status = new Status { StatusName = req.StatusName };
        await repo.AddAsync(status);
        return MapToResponse(status);
    }

    public async Task DeleteAsync(int id)
    {
        var status = await repo.GetByIdAsync(id);
        if (status is null || status.IsDeleted)
            throw new KeyNotFoundException($"Статус {id} не найден");
        status.IsDeleted = true;
        await repo.UpdateAsync(status);
    }

    private static StatusResponse MapToResponse(Status s) => new(s.StatusId, s.StatusName);
}
