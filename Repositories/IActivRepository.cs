using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IActivRepository : IGenericRepository<Activ>
{
	public IQueryable<Activ> QueryActive();
	public Task AddDrugsAsync(IEnumerable<ActivDrug> drugs);
	public Task LinkDrugAsync(int activId, int drugId);
	public Task UnlinkDrugAsync(int activId, int drugId);
}
