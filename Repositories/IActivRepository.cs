using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IActivRepository
{
    public IQueryable<Activ> QueryActive();
    public IQueryable<Activ> Query();
    public Task<Activ> AddAsync(Activ entity);
    public Task<Activ> AddWithDrugsAsync(Activ entity, IEnumerable<int> drugIds);
    public Task UpdateAsync(Activ entity);
    public Task AddDrugsAsync(IEnumerable<ActivDrug> drugs);
    public Task LinkDrugAsync(int activId, int drugId);
    public Task UnlinkDrugAsync(int activId, int drugId);
}
