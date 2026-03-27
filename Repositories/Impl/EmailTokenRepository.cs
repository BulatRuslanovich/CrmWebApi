using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class EmailTokenRepository(AppDbContext db) : GenericRepository<EmailToken>(db), IEmailTokenRepository
{
	public Task<EmailToken?> GetValidTokenAsync(string tokenHash, int tokenType) =>
		_dbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.TokenType == tokenType);

	public async Task DeleteAllForUserAsync(int usrId, int tokenType)
	{
		var tokens = await _dbSet.Where(t => t.UsrId == usrId && t.TokenType == tokenType).ToListAsync();
		_dbSet.RemoveRange(tokens);
		await _db.SaveChangesAsync();
	}
}
