using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.OrgType;
using CrmWebApi.Repositories;

namespace CrmWebApi.Services.Impl;

public class OrgTypeService(IGenericRepository<OrgType> repo) : IOrgTypeService
{
    public async Task<IEnumerable<OrgTypeResponse>> GetAllAsync() =>
        (await repo.GetAllAsync()).Select(MapToResponse);

    public async Task<OrgTypeResponse> GetByIdAsync(int id)
    {
        var orgType = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Тип организации {id} не найден");
        return MapToResponse(orgType);
    }

    public async Task<OrgTypeResponse> CreateAsync(CreateOrgTypeRequest req)
    {
        var orgType = new OrgType { OrgTypeName = req.OrgTypeName };
        await repo.AddAsync(orgType);
        return MapToResponse(orgType);
    }

    public async Task DeleteAsync(int id)
    {
        var orgType = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Тип организации {id} не найден");
        await repo.DeleteAsync(orgType);
    }

    private static OrgTypeResponse MapToResponse(OrgType o) => new(o.OrgTypeId, o.OrgTypeName);
}
