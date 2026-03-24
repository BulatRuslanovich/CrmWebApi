using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IActivRepository : IGenericRepository<Activ>
{
    IQueryable<Activ> QueryActive();
    Task AddDrugsAsync(IEnumerable<ActivDrug> drugs);
    Task AddDrugAsync(int activId, int drugId);
    Task RemoveDrugAsync(int activId, int drugId);
}
