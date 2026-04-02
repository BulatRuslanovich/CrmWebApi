using CrmWebApi.Data;
using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Repositories.Impl;

public class EmailTokenRepository(AppDbContext db) : IEmailTokenRepository
{
    public async Task<EmailToken> AddAsync(EmailToken entity)
    {
        db.EmailTokens.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(EmailToken entity)
    {
        db.EmailTokens.Remove(entity);
        await db.SaveChangesAsync();
    }

    public Task<EmailToken?> GetValidTokenAsync(string tokenHash, int tokenType) =>
        db.EmailTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.TokenType == tokenType);

    public async Task DeleteAllForUserAsync(int usrId, int tokenType)
    {
        var tokens = await db.EmailTokens.Where(t => t.UsrId == usrId && t.TokenType == tokenType).ToListAsync();
        db.EmailTokens.RemoveRange(tokens);
        await db.SaveChangesAsync();
    }
}
