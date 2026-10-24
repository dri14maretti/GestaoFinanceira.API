using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class MonthlyPlanRepository : IMonthlyPlanRepository
{
    private readonly FinanceDbContext _context;

    public MonthlyPlanRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MonthlyPlan>> GetAllAsync()
    {
        return await _context.MonthlyPlans.AsNoTracking().ToListAsync();
    }

    public async Task<MonthlyPlan?> GetByIdAsync(Guid id)
    {
        return await _context.MonthlyPlans.FindAsync(id);
    }

    public async Task<MonthlyPlan?> GetByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.MonthlyPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Month >= startOfMonth && p.Month < endOfMonth);
    }

    public async Task<MonthlyPlan> AddAsync(MonthlyPlan entity)
    {
        await _context.MonthlyPlans.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<MonthlyPlan?> UpdateAsync(MonthlyPlan entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.MonthlyPlans.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.MonthlyPlans.FindAsync(id);
        if (entity == null) return false;

        _context.MonthlyPlans.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
