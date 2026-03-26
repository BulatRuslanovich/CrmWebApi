using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class ActivRepository(AppDbContext db) : GenericRepository<Activ>(db), IActivRepository
{
	public IQueryable<Activ> QueryActive() =>
		_dbSet
			.Include(a => a.Usr)
			.Include(a => a.Org)
			.Include(a => a.Status)
			.Include(a => a.ActivDrugs).ThenInclude(ad => ad.Drug)
			.Where(a => !a.IsDeleted);

	public async Task AddDrugsAsync(IEnumerable<ActivDrug> drugs)
	{
		await _db.ActivDrugs.AddRangeAsync(drugs);
		await _db.SaveChangesAsync();
	}

	public async Task LinkDrugAsync(int activId, int drugId)
	{
		_db.ActivDrugs.Add(new ActivDrug { ActivId = activId, DrugId = drugId });
		await _db.SaveChangesAsync();
	}

	public async Task UnlinkDrugAsync(int activId, int drugId)
	{
		var link = await _db.ActivDrugs.FirstOrDefaultAsync(ad => ad.ActivId == activId && ad.DrugId == drugId)
			?? throw new KeyNotFoundException("Связь не найдена");
		_db.ActivDrugs.Remove(link);
		await _db.SaveChangesAsync();
	}
}
