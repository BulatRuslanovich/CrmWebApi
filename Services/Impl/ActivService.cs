using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Activ;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Services.Impl;

public class ActivService(IActivRepository repo) : IActivService
{
	public async Task<PagedResponse<ActivResponse>> GetAllAsync(int page, int pageSize, int? usrId = null)
	{
		var query = repo.QueryActive();
		if (usrId is not null)
			query = query.Where(a => a.UsrId == usrId);

		var total = await query.CountAsync();
		var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
			.Select(MapToResponse).ToList();
		return new PagedResponse<ActivResponse>(items, page, pageSize, total);
	}

	public async Task<ActivResponse> GetByIdAsync(int id)
	{
		var activ = await repo.QueryActive().FirstOrDefaultAsync(a => a.ActivId == id)
			?? throw new KeyNotFoundException($"Активность {id} не найдена");
		return MapToResponse(activ);
	}

	public async Task<ActivResponse> CreateAsync(int usrId, CreateActivRequest req)
	{
		var activ = new Activ
		{
			UsrId = usrId,
			OrgId = req.OrgId,
			StatusId = req.StatusId,
			ActivStart = req.Start,
			ActivEnd = req.End,
			ActivDescription = req.Description,
			ActivResult = req.Result
		};
		await repo.AddAsync(activ);

		var drugs = req.DrugIds.Distinct()
			.Select(drugId => new ActivDrug { ActivId = activ.ActivId, DrugId = drugId });
		await repo.AddDrugsAsync(drugs);

		return await GetByIdAsync(activ.ActivId);
	}

	public async Task<ActivResponse> UpdateAsync(int id, UpdateActivRequest req)
	{
		var activ = await repo.Query().FirstOrDefaultAsync(a => a.ActivId == id && !a.IsDeleted)
			?? throw new KeyNotFoundException($"Активность {id} не найдена");

		activ.StatusId = req.StatusId ?? activ.StatusId;
		activ.ActivStart = req.Start ?? activ.ActivStart;
		activ.ActivEnd = req.End ?? activ.ActivEnd;
		activ.ActivDescription = req.Description ?? activ.ActivDescription;
		activ.ActivResult = req.Result ?? activ.ActivResult;

		await repo.UpdateAsync(activ);
		return await GetByIdAsync(id);
	}

	public async Task DeleteAsync(int id)
	{
		var activ = await repo.Query().FirstOrDefaultAsync(a => a.ActivId == id && !a.IsDeleted)
			?? throw new KeyNotFoundException($"Активность {id} не найдена");
		activ.IsDeleted = true;
		await repo.UpdateAsync(activ);
	}

	public async Task LinkDrugAsync(int activId, int drugId) =>
		await repo.LinkDrugAsync(activId, drugId);

	public async Task UnlinkDrugAsync(int activId, int drugId) =>
		await repo.UnlinkDrugAsync(activId, drugId);

	private static ActivResponse MapToResponse(Activ a) => new(
		a.ActivId, a.UsrId, a.Usr.UsrLogin,
		a.OrgId, a.Org.OrgName,
		a.StatusId, a.Status.StatusName,
		a.ActivStart, a.ActivEnd,
		a.ActivDescription, a.ActivResult,
		[.. a.ActivDrugs.Where(ad => !ad.Drug.IsDeleted).Select(ad => ad.Drug.DrugName)]
	);
}
