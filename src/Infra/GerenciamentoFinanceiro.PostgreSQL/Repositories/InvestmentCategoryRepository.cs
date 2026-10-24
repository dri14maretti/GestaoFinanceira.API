using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class InvestmentCategoryRepository : IInvestmentCategoryRepository
{
    private readonly FinanceDbContext _context;

    public InvestmentCategoryRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvestmentCategory>> GetAllAsync()
    {
        return await _context.InvestmentCategories.AsNoTracking().ToListAsync();
    }

    public async Task<InvestmentCategory?> GetByIdAsync(Guid id)
    {
        return await _context.InvestmentCategories.FindAsync(id);
    }

    public async Task<InvestmentCategory> AddAsync(InvestmentCategory entity)
    {
        await _context.InvestmentCategories.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<InvestmentCategory?> UpdateAsync(InvestmentCategory entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.InvestmentCategories.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.InvestmentCategories.FindAsync(id);
        if (entity == null) return false;

        _context.InvestmentCategories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
