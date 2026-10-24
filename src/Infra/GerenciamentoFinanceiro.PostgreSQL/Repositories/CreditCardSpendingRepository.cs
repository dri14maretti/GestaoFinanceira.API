using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class CreditCardSpendingRepository : ICreditCardSpendingRepository
{
    private readonly FinanceDbContext _context;

    public CreditCardSpendingRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CreditCardSpending>> GetAllAsync()
    {
        return await _context.CreditCardSpendings.AsNoTracking().ToListAsync();
    }

    public async Task<CreditCardSpending?> GetByIdAsync(Guid id)
    {
        return await _context.CreditCardSpendings.FindAsync(id);
    }

    public async Task<IEnumerable<CreditCardSpending>> GetByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.CreditCardSpendings
            .AsNoTracking()
            .Where(s => s.Date >= startOfMonth && s.Date < endOfMonth)
            .ToListAsync();
    }

    public async Task<IEnumerable<CreditCardSpending>> GetByCreditCardIdAsync(Guid creditCardId)
    {
        return await _context.CreditCardSpendings
            .AsNoTracking()
            .Where(s => s.CreditCardId == creditCardId)
            .ToListAsync();
    }

    public async Task<CreditCardSpending> AddAsync(CreditCardSpending entity)
    {
        await _context.CreditCardSpendings.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<CreditCardSpending?> UpdateAsync(CreditCardSpending entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.CreditCardSpendings.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.CreditCardSpendings.FindAsync(id);
        if (entity == null) return false;

        _context.CreditCardSpendings.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.CreditCardSpendings
            .Where(s => s.Date >= startOfMonth && s.Date < endOfMonth)
            .SumAsync(s => (decimal?)s.Value) ?? 0m;
    }

    public async Task<Dictionary<Guid, decimal>> GetTotalsByCategoryAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.CreditCardSpendings
            .Where(s => s.Date >= startOfMonth && s.Date < endOfMonth)
            .GroupBy(s => s.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(s => s.Value) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total);
    }
}
