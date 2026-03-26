
using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IRefreshRepository : IGenericRepository<Refresh>
{
	public Task<Refresh?> GetByTokenHashAsync(string tokenHash);
	public Task RevokeAllForUserAsync(int usrId);
}
