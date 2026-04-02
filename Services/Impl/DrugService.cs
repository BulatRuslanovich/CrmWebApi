using CrmWebApi.Common;
using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Drug;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CrmWebApi.Services.Impl;

public class DrugService(AppDbContext db, IMemoryCache cache, ILogger<DrugService> logger) : IDrugService
{
    private CancellationTokenSource _cts = new();

    public async Task<Result<PagedResponse<DrugResponse>>> GetAllAsync(int page, int pageSize)
    {
        var cacheKey = $"drugs_{page}_{pageSize}";
        if (cache.TryGetValue(cacheKey, out PagedResponse<DrugResponse>? cached))
            return cached!;

        var query = db.Drugs.AsQueryable();
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var result = new PagedResponse<DrugResponse>(items.Select(MapToResponse).ToList(), page, pageSize, total);

        var options = new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(_cts.Token))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        cache.Set(cacheKey, result, options);
        return result;
    }

    public async Task<Result<DrugResponse>> GetByIdAsync(int id)
    {
        var drug = await db.Drugs.FirstOrDefaultAsync(d => d.DrugId == id);
        if (drug is null)
            return Error.NotFound($"Препарат {id} не найден");
        return MapToResponse(drug);
    }

    public async Task<Result<DrugResponse>> CreateAsync(CreateDrugRequest req)
    {
        var drug = new Drug
        {
            DrugName = req.DrugName,
            DrugBrand = req.Brand,
            DrugForm = req.Form,
            DrugDescription = req.Description
        };
        db.Drugs.Add(drug);
        await db.SaveChangesAsync();
        Invalidate();
        logger.LogInformation("Препарат создан: {DrugName} (id={DrugId})", drug.DrugName, drug.DrugId);
        return MapToResponse(drug);
    }

    public async Task<Result<DrugResponse>> UpdateAsync(int id, UpdateDrugRequest req)
    {
        var drug = await db.Drugs.FirstOrDefaultAsync(d => d.DrugId == id);
        if (drug is null)
            return Error.NotFound($"Препарат {id} не найден");

        drug.DrugName = req.DrugName ?? drug.DrugName;
        drug.DrugBrand = req.Brand ?? drug.DrugBrand;
        drug.DrugForm = req.Form ?? drug.DrugForm;
        drug.DrugDescription = req.Description ?? drug.DrugDescription;

        await db.SaveChangesAsync();
        Invalidate();
        logger.LogInformation("Препарат обновлён: id={DrugId}", id);
        return MapToResponse(drug);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var drug = await db.Drugs.FirstOrDefaultAsync(d => d.DrugId == id);
        if (drug is null)
            return Error.NotFound($"Препарат {id} не найден");

        drug.IsDeleted = true;
        await db.SaveChangesAsync();
        Invalidate();
        logger.LogInformation("Препарат удалён: id={DrugId}", id);
        return Result.Success();
    }

    private void Invalidate()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
    }

    private static DrugResponse MapToResponse(Drug d) =>
        new(d.DrugId, d.DrugName, d.DrugBrand, d.DrugForm, d.DrugDescription);
}
