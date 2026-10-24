using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class FixedExpenseRepository : IFixedExpenseRepository
{
    private readonly FinanceDbContext _context;

    public FixedExpenseRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FixedExpense>> GetAllAsync()
    {
        return await _context.FixedExpenses.AsNoTracking().ToListAsync();
    }

    public async Task<FixedExpense?> GetByIdAsync(Guid id)
    {
        return await _context.FixedExpenses.FindAsync(id);
    }

    public async Task<IEnumerable<FixedExpense>> GetByCreditCardIdAsync(Guid creditCardId)
    {
        return await _context.FixedExpenses
            .AsNoTracking()
            .Where(f => f.CreditCardId == creditCardId)
            .ToListAsync();
    }

    public async Task<FixedExpense> AddAsync(FixedExpense entity)
    {
        await _context.FixedExpenses.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<FixedExpense?> UpdateAsync(FixedExpense entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.FixedExpenses.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.FixedExpenses.FindAsync(id);
        if (entity == null) return false;

        _context.FixedExpenses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalAsync()
    {
        return await _context.FixedExpenses.SumAsync(f => (decimal?)f.Value) ?? 0m;
    }
}
