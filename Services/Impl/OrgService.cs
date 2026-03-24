using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs.Org;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Services.Impl;

public class OrgService(IOrgRepository repo) : IOrgService
{
    public async Task<IEnumerable<OrgResponse>> GetAllAsync() =>
        await repo.QueryActive().Select(o => MapToResponse(o)).ToListAsync();

    public async Task<OrgResponse> GetByIdAsync(int id)
    {
        var org = await repo.QueryActive().FirstOrDefaultAsync(o => o.OrgId == id)
            ?? throw new KeyNotFoundException($"Организация {id} не найдена");
        return MapToResponse(org);
    }

    public async Task<OrgResponse> CreateAsync(CreateOrgRequest req)
    {
        var org = new Org
        {
            OrgTypeId    = req.OrgTypeId,
            OrgName      = req.OrgName,
            OrgInn       = req.Inn,
            OrgLatitude  = req.Latitude,
            OrgLongitude = req.Longitude,
            OrgAddress   = req.Address
        };
        await repo.AddAsync(org);
        return await GetByIdAsync(org.OrgId);
    }

    public async Task<OrgResponse> UpdateAsync(int id, UpdateOrgRequest req)
    {
        var org = await repo.Query().FirstOrDefaultAsync(o => o.OrgId == id && !o.IsDeleted)
            ?? throw new KeyNotFoundException($"Организация {id} не найдена");

        org.OrgTypeId    = req.OrgTypeId    ?? org.OrgTypeId;
        org.OrgName      = req.OrgName      ?? org.OrgName;
        org.OrgInn       = req.Inn          ?? org.OrgInn;
        org.OrgLatitude  = req.Latitude     ?? org.OrgLatitude;
        org.OrgLongitude = req.Longitude    ?? org.OrgLongitude;
        org.OrgAddress   = req.Address      ?? org.OrgAddress;

        await repo.UpdateAsync(org);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var org = await repo.Query().FirstOrDefaultAsync(o => o.OrgId == id && !o.IsDeleted)
            ?? throw new KeyNotFoundException($"Организация {id} не найдена");
        org.IsDeleted = true;
        await repo.UpdateAsync(org);
    }

    private static OrgResponse MapToResponse(Org o) => new(
        o.OrgId, o.OrgTypeId, o.OrgType.OrgTypeName,
        o.OrgName, o.OrgInn, o.OrgLatitude, o.OrgLongitude, o.OrgAddress
    );
}
