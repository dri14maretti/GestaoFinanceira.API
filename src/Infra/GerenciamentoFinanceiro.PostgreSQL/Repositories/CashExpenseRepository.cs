using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class CashExpenseRepository : ICashExpenseRepository
{
    private readonly FinanceDbContext _context;

    public CashExpenseRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CashExpense>> GetAllAsync()
    {
        return await _context.CashExpenses.AsNoTracking().ToListAsync();
    }

    public async Task<CashExpense?> GetByIdAsync(Guid id)
    {
        return await _context.CashExpenses.FindAsync(id);
    }

    public async Task<IEnumerable<CashExpense>> GetByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.CashExpenses
            .AsNoTracking()
            .Where(e => e.Date >= startOfMonth && e.Date < endOfMonth)
            .ToListAsync();
    }

    public async Task<IEnumerable<CashExpense>> GetByBankAsync(string bank)
    {
        return await _context.CashExpenses
            .AsNoTracking()
            .Where(e => EF.Functions.ILike(e.Bank, bank))
            .ToListAsync();
    }

    public async Task<CashExpense> AddAsync(CashExpense entity)
    {
        await _context.CashExpenses.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<CashExpense?> UpdateAsync(CashExpense entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.CashExpenses.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.CashExpenses.FindAsync(id);
        if (entity == null) return false;

        _context.CashExpenses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        // Sum negative values as positive expenses; also support positive legacy expense values
        var expenses = await _context.CashExpenses
            .Where(e => e.Date >= startOfMonth && e.Date < endOfMonth)
            .ToListAsync();

        return expenses.Sum(e => e.Value < 0 ? Math.Abs(e.Value) : e.Value);
    }

    public async Task<Dictionary<Guid, decimal>> GetTotalsByCategoryAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var expenses = await _context.CashExpenses
            .Where(e => e.Date >= startOfMonth && e.Date < endOfMonth && e.CategoryId.HasValue)
            .ToListAsync();

        return expenses
            .GroupBy(e => e.CategoryId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.Value < 0 ? Math.Abs(e.Value) : e.Value)
            );
    }

    public async Task<Dictionary<string, decimal>> GetBalancesByBankAsync()
    {
        return await _context.CashExpenses
            .GroupBy(e => e.Bank)
            .Select(g => new { Bank = g.Key, Total = g.Sum(e => e.Value) })
            .ToDictionaryAsync(x => x.Bank, x => x.Total);
    }
}
