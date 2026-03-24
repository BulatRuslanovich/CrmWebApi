
using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IRefreshRepository : IGenericRepository<Refresh>
{
    Task<Refresh?> GetByTokenHashAsync(string tokenHash);
    Task RevokeAllForUserAsync(int usrId);
}