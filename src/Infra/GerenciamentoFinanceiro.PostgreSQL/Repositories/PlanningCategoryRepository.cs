using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class PlanningCategoryRepository : IPlanningCategoryRepository
{
    private readonly FinanceDbContext _context;

    public PlanningCategoryRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlanningCategory>> GetAllAsync()
    {
        return await _context.PlanningCategories.AsNoTracking().ToListAsync();
    }

    public async Task<PlanningCategory?> GetByIdAsync(Guid id)
    {
        return await _context.PlanningCategories.FindAsync(id);
    }

    public async Task<PlanningCategory> AddAsync(PlanningCategory entity)
    {
        await _context.PlanningCategories.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PlanningCategory?> UpdateAsync(PlanningCategory entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.PlanningCategories.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.PlanningCategories.FindAsync(id);
        if (entity == null) return false;

        _context.PlanningCategories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
