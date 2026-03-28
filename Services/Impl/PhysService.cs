using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Phys;
using CrmWebApi.DTOs.Spec;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CrmWebApi.Services.Impl;

public class PhysService(IPhysRepository repo, IGenericRepository<Spec> specRepo, IMemoryCache cache, ILogger<PhysService> logger) : IPhysService
{
	private CancellationTokenSource _cts = new();
	public async Task<PagedResponse<PhysResponse>> GetAllAsync(int page, int pageSize)
	{
		var query = repo.QueryActive();
		var total = await query.CountAsync();
		var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
			.Select(MapToResponse).ToList();
		return new PagedResponse<PhysResponse>(items, page, pageSize, total);
	}

	public async Task<PhysResponse> GetByIdAsync(int id)
	{
		var phys = await repo.QueryActive().FirstOrDefaultAsync(p => p.PhysId == id)
			?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");
		return MapToResponse(phys);
	}

	public async Task<PhysResponse> CreateAsync(CreatePhysRequest req)
	{
		var phys = new Phys
		{
			SpecId = req.SpecId,
			PhysFirstname = req.FirstName,
			PhysLastname = req.LastName,
			PhysMiddlename = req.MiddleName,
			PhysPhone = req.Phone,
			PhysEmail = req.Email,
			PhysPosition = req.Position
		};
		await repo.AddAsync(phys);
		logger.LogInformation("Физлицо создано: id={PhysId}", phys.PhysId);
		return await GetByIdAsync(phys.PhysId);
	}

	public async Task<PhysResponse> UpdateAsync(int id, UpdatePhysRequest req)
	{
		var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id)
			?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");

		phys.SpecId = req.SpecId ?? phys.SpecId;
		phys.PhysFirstname = req.FirstName ?? phys.PhysFirstname;
		phys.PhysLastname = req.LastName ?? phys.PhysLastname;
		phys.PhysMiddlename = req.MiddleName ?? phys.PhysMiddlename;
		phys.PhysPhone = req.Phone ?? phys.PhysPhone;
		phys.PhysEmail = req.Email ?? phys.PhysEmail;
		phys.PhysPosition = req.Position ?? phys.PhysPosition;

		await repo.UpdateAsync(phys);
		logger.LogInformation("Физлицо обновлено: id={PhysId}", id);
		return await GetByIdAsync(id);
	}

	public async Task DeleteAsync(int id)
	{
		var phys = await repo.Query().FirstOrDefaultAsync(p => p.PhysId == id)
			?? throw new KeyNotFoundException($"Физическое лицо {id} не найдено");
		phys.IsDeleted = true;
		await repo.UpdateAsync(phys);
		logger.LogInformation("Физлицо удалено: id={PhysId}", id);
	}

	public async Task LinkOrgAsync(int physId, int orgId) =>
		await repo.LinkOrgAsync(physId, orgId);

	public async Task UnlinkOrgAsync(int physId, int orgId) =>
		await repo.UnlinkOrgAsync(physId, orgId);

	private static PhysResponse MapToResponse(Phys p) => new(
		p.PhysId, p.SpecId, p.Spec?.SpecName,
		p.PhysFirstname, p.PhysLastname, p.PhysMiddlename,
		p.PhysPhone, p.PhysEmail, p.PhysPosition,
		[.. p.PhysOrgs.Where(po => po.Org is not null).Select(po => po.Org.OrgName)]
	);

	private const string AllKey = "specs";

	public async Task<IEnumerable<SpecResponse>> GetAllSpecsAsync()
	{
		if (cache.TryGetValue(AllKey, out IEnumerable<SpecResponse>? cached))
			return cached!;
		var result = (await specRepo.GetAllAsync()).Select(MapToResponse).ToList();
		var options = new MemoryCacheEntryOptions()
			.AddExpirationToken(new CancellationChangeToken(_cts.Token))
			.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
		cache.Set(AllKey, result, options);
		return result;
	}

	public async Task<SpecResponse> GetSpecByIdAsync(int id)
	{
		var spec = await specRepo.Query().FirstOrDefaultAsync(s => s.SpecId == id)
			?? throw new KeyNotFoundException($"Специальность {id} не найдена");
		return MapToResponse(spec);
	}

	public async Task<SpecResponse> CreateSpecAsync(CreateSpecRequest req)
	{
		var spec = new Spec { SpecName = req.SpecName };
		await specRepo.AddAsync(spec);
		InvalidateCache();
		logger.LogInformation("Специальность создана: {SpecName} (id={SpecId})", spec.SpecName, spec.SpecId);
		return MapToResponse(spec);
	}

	public async Task DeleteSpecAsync(int id)
	{
		var spec = await specRepo.Query().FirstOrDefaultAsync(s => s.SpecId == id)
			?? throw new KeyNotFoundException($"Специальность {id} не найдена");
		spec.IsDeleted = true;
		await specRepo.UpdateAsync(spec);
		InvalidateCache();
		logger.LogInformation("Специальность удалена: id={SpecId}", id);
	}

	private void InvalidateCache()
	{
		_cts.Cancel();
		_cts = new CancellationTokenSource();
	}

	private static SpecResponse MapToResponse(Spec s) => new(s.SpecId, s.SpecName);
}
