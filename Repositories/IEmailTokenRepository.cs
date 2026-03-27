using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IEmailTokenRepository : IGenericRepository<EmailToken>
{
	Task<EmailToken?> GetValidTokenAsync(string tokenHash, int tokenType);
	Task DeleteAllForUserAsync(int usrId, int tokenType);
}
