using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IActivRepository : IGenericRepository<Activ>
{
    IQueryable<Activ> QueryActive();
    Task AddDrugsAsync(IEnumerable<ActivDrug> drugs);
    Task LinkDrugAsync(int activId, int drugId);
    Task UnlinkDrugAsync(int activId, int drugId);
}
