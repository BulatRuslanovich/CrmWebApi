
using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class RefreshRepository(AppDbContext db) : GenericRepository<Refresh>(db), IRefreshRepository
{
    public Task<Refresh?> GetByTokenHashAsync(string tokenHash) =>
        _dbSet.Include(r => r.Usr)
                .FirstOrDefaultAsync(r => r.RefreshTokenHash == tokenHash);

    public async Task RevokeAllForUserAsync(int usrId)
    {
        var tokens = await _dbSet.Where(r => r.UsrId == usrId).ToListAsync();
        _dbSet.RemoveRange(tokens);
        await _db.SaveChangesAsync();
    }
}