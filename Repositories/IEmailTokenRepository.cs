using CrmWebApi.Data.Entities;

namespace CrmWebApi.Repositories;

public interface IEmailTokenRepository
{
    public Task<EmailToken> AddAsync(EmailToken entity);
    public Task DeleteAsync(EmailToken entity);
    public Task<EmailToken?> GetValidTokenAsync(string tokenHash, int tokenType);
    public Task DeleteAllForUserAsync(int usrId, int tokenType);
}
