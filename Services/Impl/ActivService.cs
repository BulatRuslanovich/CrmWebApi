using CrmWebApi.Common;
using CrmWebApi.Data.Entities;
using CrmWebApi.DTOs;
using CrmWebApi.DTOs.Activ;
using CrmWebApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Services.Impl;

public class ActivService(IActivRepository repo, ILogger<ActivService> logger) : IActivService
{
    public async Task<Result<PagedResponse<ActivResponse>>> GetAllAsync(int page, int pageSize, int? usrId = null)
    {
        var query = repo.QueryActive();
        if (usrId is not null)
            query = query.Where(a => a.UsrId == usrId);

        var total = await query.CountAsync();
        var items = (await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(MapToResponse).ToList();
        return new PagedResponse<ActivResponse>(items, page, pageSize, total);
    }

    public async Task<Result<ActivResponse>> GetByIdAsync(int id)
    {
        var activ = await repo.QueryActive().FirstOrDefaultAsync(a => a.ActivId == id);
        if (activ is null)
            return Error.NotFound($"Активность {id} не найдена");
        return MapToResponse(activ);
    }

    public async Task<Result<ActivResponse>> CreateAsync(int usrId, CreateActivRequest req)
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

        logger.LogInformation("Activity created: id={ActivId}, usr={UsrId}", activ.ActivId, usrId);
        return await GetByIdAsync(activ.ActivId);
    }

    public async Task<Result<ActivResponse>> UpdateAsync(int id, UpdateActivRequest req)
    {
        var activ = await repo.Query().FirstOrDefaultAsync(a => a.ActivId == id);
        if (activ is null)
            return Error.NotFound($"Активность {id} не найдена");

        activ.StatusId = req.StatusId ?? activ.StatusId;
        activ.ActivStart = req.Start ?? activ.ActivStart;
        activ.ActivEnd = req.End ?? activ.ActivEnd;
        activ.ActivDescription = req.Description ?? activ.ActivDescription;
        activ.ActivResult = req.Result ?? activ.ActivResult;

        await repo.UpdateAsync(activ);
        logger.LogInformation("Activity updated: id={ActivId}", id);
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var activ = await repo.Query().FirstOrDefaultAsync(a => a.ActivId == id);
        if (activ is null)
            return Error.NotFound($"Активность {id} не найдена");

        activ.IsDeleted = true;
        await repo.UpdateAsync(activ);
        logger.LogInformation("Activity deleted: id={ActivId}", id);
        return Result.Success();
    }

    public async Task<Result> LinkDrugAsync(int activId, int drugId)
    {
        await repo.LinkDrugAsync(activId, drugId);
        return Result.Success();
    }

    public async Task<Result> UnlinkDrugAsync(int activId, int drugId)
    {
        await repo.UnlinkDrugAsync(activId, drugId);
        return Result.Success();
    }

    private static ActivResponse MapToResponse(Activ a) => new(
        a.ActivId, a.UsrId, a.Usr.UsrLogin,
        a.OrgId, a.Org.OrgName,
        a.StatusId, a.Status.StatusName,
        a.ActivStart, a.ActivEnd,
        a.ActivDescription, a.ActivResult,
        [.. a.ActivDrugs.Where(ad => ad.Drug is not null).Select(ad => ad.Drug.DrugName)]
    );
}
