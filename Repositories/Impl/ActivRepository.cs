using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class ActivRepository(AppDbContext db) : IActivRepository
{
    public IQueryable<Activ> QueryActive() =>
        db.Activs
            .Include(a => a.Usr)
            .Include(a => a.Org)
            .Include(a => a.Status)
            .Include(a => a.ActivDrugs).ThenInclude(ad => ad.Drug);

    public IQueryable<Activ> Query() => db.Activs.AsQueryable();

    public async Task<Activ> AddAsync(Activ entity)
    {
        db.Activs.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Activ entity)
    {
        db.Activs.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task AddDrugsAsync(IEnumerable<ActivDrug> drugs)
    {
        await db.ActivDrugs.AddRangeAsync(drugs);
        await db.SaveChangesAsync();
    }

    public async Task LinkDrugAsync(int activId, int drugId)
    {
        db.ActivDrugs.Add(new ActivDrug { ActivId = activId, DrugId = drugId });
        await db.SaveChangesAsync();
    }

    public async Task UnlinkDrugAsync(int activId, int drugId)
    {
        var link = await db.ActivDrugs.FirstOrDefaultAsync(ad => ad.ActivId == activId && ad.DrugId == drugId)
            ?? throw new KeyNotFoundException("Связь не найдена");
        db.ActivDrugs.Remove(link);
        await db.SaveChangesAsync();
    }
}
