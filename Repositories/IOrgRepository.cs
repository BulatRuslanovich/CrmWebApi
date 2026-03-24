using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IOrgRepository : IGenericRepository<Org>
{
    IQueryable<Org> QueryActive();
}
