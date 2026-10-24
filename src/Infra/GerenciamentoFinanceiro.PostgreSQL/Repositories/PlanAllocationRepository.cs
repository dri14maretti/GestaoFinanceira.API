using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class PlanAllocationRepository : IPlanAllocationRepository
{
    private readonly FinanceDbContext _context;

    public PlanAllocationRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanAllocation>> GetAllAsync()
    {
        return await _context.PlanAllocations.AsNoTracking().ToListAsync();
    }

    public async Task<PlanAllocation?> GetByIdAsync(Guid id)
    {
        return await _context.PlanAllocations.FindAsync(id);
    }

    public async Task<IEnumerable<PlanAllocation>> GetByPlanIdAsync(Guid monthlyPlanId)
    {
        return await _context.PlanAllocations
            .AsNoTracking()
            .Where(a => a.MonthlyPlanId == monthlyPlanId)
            .ToListAsync();
    }

    public async Task<PlanAllocation> AddAsync(PlanAllocation entity)
    {
        await _context.PlanAllocations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PlanAllocation?> UpdateAsync(PlanAllocation entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.PlanAllocations.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.PlanAllocations.FindAsync(id);
        if (entity == null) return false;

        _context.PlanAllocations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalAllocatedByPlanIdAsync(Guid monthlyPlanId)
    {
        return await _context.PlanAllocations
            .Where(a => a.MonthlyPlanId == monthlyPlanId)
            .SumAsync(a => (decimal?)a.AllocatedValue) ?? 0m;
    }
}
