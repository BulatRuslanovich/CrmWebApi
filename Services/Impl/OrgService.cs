using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Org;
using CrmWebApi.DTOs.OrgType;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CrmWebApi.Services.Impl;

public class OrgService(IOrgRepository repo, IMemoryCache cache) : IOrgService
{
    private const string OrgTypesKey = "org_types";

    public async Task<PagedResponse<OrgResponse>> GetAllAsync(int page, int pageSize)
    {
        var query = repo.QueryActive();
        var total = await query.CountAsync();
        var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(MapToResponse).ToList();
        return new PagedResponse<OrgResponse>(items, page, pageSize, total);
    }

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

    public async Task<IEnumerable<OrgTypeResponse>> GetAllTypesAsync()
    {
        if (cache.TryGetValue(OrgTypesKey, out IEnumerable<OrgTypeResponse>? cached))
            return cached!;
        var result = await repo.QueryOrgTypes()
            .Select(ot => new OrgTypeResponse(ot.OrgTypeId, ot.OrgTypeName))
            .ToListAsync();
        cache.Set(OrgTypesKey, result, TimeSpan.FromMinutes(10));
        return result;
    }
}
