using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class InstallmentRepository : IInstallmentRepository
{
    private readonly FinanceDbContext _context;

    public InstallmentRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Installment>> GetAllAsync()
    {
        return await _context.Installments.AsNoTracking().ToListAsync();
    }

    public async Task<Installment?> GetByIdAsync(Guid id)
    {
        return await _context.Installments.FindAsync(id);
    }

    public async Task<IEnumerable<Installment>> GetByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.Installments
            .AsNoTracking()
            .Where(i => i.Month >= startOfMonth && i.Month < endOfMonth)
            .ToListAsync();
    }

    public async Task<IEnumerable<Installment>> GetByCreditCardIdAsync(Guid creditCardId)
    {
        return await _context.Installments
            .AsNoTracking()
            .Where(i => i.CreditCardId == creditCardId)
            .ToListAsync();
    }

    public async Task<Installment> AddAsync(Installment entity)
    {
        await _context.Installments.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Installment?> UpdateAsync(Installment entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.Installments.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Installments.FindAsync(id);
        if (entity == null) return false;

        _context.Installments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.Installments
            .Where(i => i.Month >= startOfMonth && i.Month < endOfMonth)
            .SumAsync(i => (decimal?)i.Value) ?? 0m;
    }

    public async Task<Dictionary<Guid, decimal>> GetTotalsByCategoryAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.Installments
            .Where(i => i.Month >= startOfMonth && i.Month < endOfMonth)
            .GroupBy(i => i.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(i => i.Value) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total);
    }
}
