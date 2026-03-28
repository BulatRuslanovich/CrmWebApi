using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IEmailTokenRepository : IGenericRepository<EmailToken>
{
	public Task<EmailToken?> GetValidTokenAsync(string tokenHash, int tokenType);
	public Task DeleteAllForUserAsync(int usrId, int tokenType);
}
