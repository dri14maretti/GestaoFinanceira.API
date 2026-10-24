using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class InvestmentRepository : IInvestmentRepository
{
    private readonly FinanceDbContext _context;

    public InvestmentRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Investment>> GetAllAsync()
    {
        return await _context.Investments.AsNoTracking().ToListAsync();
    }

    public async Task<Investment?> GetByIdAsync(Guid id)
    {
        return await _context.Investments.FindAsync(id);
    }

    public async Task<Investment> AddAsync(Investment entity)
    {
        await _context.Investments.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Investment?> UpdateAsync(Investment entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.Investments.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Investments.FindAsync(id);
        if (entity == null) return false;

        _context.Investments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalValueAsync()
    {
        return await _context.Investments.SumAsync(i => (decimal?)i.Value) ?? 0m;
    }
}
