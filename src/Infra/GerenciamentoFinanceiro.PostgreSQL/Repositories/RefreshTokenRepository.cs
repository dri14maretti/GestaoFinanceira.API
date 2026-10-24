using System;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly FinanceDbContext _context;

    public RefreshTokenRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            return null;

        return await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        var entry = _context.Entry(refreshToken);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.RefreshTokens.FindAsync(refreshToken.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(refreshToken);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeFamilyAsync(Guid familyId, DateTime now)
    {
        var familyTokens = await _context.RefreshTokens
            .Where(r => r.FamilyId == familyId && r.RevokedAt == null)
            .ToListAsync();

        foreach (var token in familyTokens)
        {
            token.Revoke(now);
        }

        await _context.SaveChangesAsync();
    }
}
