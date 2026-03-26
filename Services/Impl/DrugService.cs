using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Drug;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CrmWebApi.Services.Impl;

public class DrugService(IGenericRepository<Drug> repo, IMemoryCache cache) : IDrugService
{
	private CancellationTokenSource _cts = new();

	public async Task<PagedResponse<DrugResponse>> GetAllAsync(int page, int pageSize)
	{
		var cacheKey = $"drugs_{page}_{pageSize}";
		if (cache.TryGetValue(cacheKey, out PagedResponse<DrugResponse>? cached))
			return cached!;

		var query = repo.Query().Where(d => !d.IsDeleted);
		var total = await query.CountAsync();
		var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
			.Select(MapToResponse).ToList();
		var result = new PagedResponse<DrugResponse>(items, page, pageSize, total);

		var options = new MemoryCacheEntryOptions()
			.AddExpirationToken(new CancellationChangeToken(_cts.Token))
			.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
		cache.Set(cacheKey, result, options);
		return result;
	}

	public async Task<DrugResponse> GetByIdAsync(int id)
	{
		var drug = await repo.GetByIdAsync(id);
		if (drug is null || drug.IsDeleted)
			throw new KeyNotFoundException($"Препарат {id} не найден");
		return MapToResponse(drug);
	}

	public async Task<DrugResponse> CreateAsync(CreateDrugRequest req)
	{
		var drug = new Drug
		{
			DrugName = req.DrugName,
			DrugBrand = req.Brand,
			DrugForm = req.Form,
			DrugDescription = req.Description
		};
		await repo.AddAsync(drug);
		Invalidate();
		return MapToResponse(drug);
	}

	public async Task<DrugResponse> UpdateAsync(int id, UpdateDrugRequest req)
	{
		var drug = await repo.GetByIdAsync(id);
		if (drug is null || drug.IsDeleted)
			throw new KeyNotFoundException($"Препарат {id} не найден");

		drug.DrugName = req.DrugName ?? drug.DrugName;
		drug.DrugBrand = req.Brand ?? drug.DrugBrand;
		drug.DrugForm = req.Form ?? drug.DrugForm;
		drug.DrugDescription = req.Description ?? drug.DrugDescription;

		await repo.UpdateAsync(drug);
		Invalidate();
		return MapToResponse(drug);
	}

	public async Task DeleteAsync(int id)
	{
		var drug = await repo.GetByIdAsync(id);
		if (drug is null || drug.IsDeleted)
			throw new KeyNotFoundException($"Препарат {id} не найден");
		drug.IsDeleted = true;
		await repo.UpdateAsync(drug);
		Invalidate();
	}

	private void Invalidate()
	{
		_cts.Cancel();
		_cts = new CancellationTokenSource();
	}

	private static DrugResponse MapToResponse(Drug d) =>
		new(d.DrugId, d.DrugName, d.DrugBrand, d.DrugForm, d.DrugDescription);
}
