using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class StoredMoneyFundRepository : IStoredMoneyFundRepository
{
    private readonly FinanceDbContext _context;

    public StoredMoneyFundRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoredMoneyFund>> GetAllAsync()
    {
        return await _context.StoredMoneyFunds.AsNoTracking().ToListAsync();
    }

    public async Task<StoredMoneyFund?> GetByIdAsync(Guid id)
    {
        return await _context.StoredMoneyFunds.FindAsync(id);
    }

    public async Task<StoredMoneyFund> AddAsync(StoredMoneyFund entity)
    {
        await _context.StoredMoneyFunds.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<StoredMoneyFund?> UpdateAsync(StoredMoneyFund entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.StoredMoneyFunds.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.StoredMoneyFunds.FindAsync(id);
        if (entity == null) return false;

        _context.StoredMoneyFunds.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalValueAsync()
    {
        return await _context.StoredMoneyFunds.SumAsync(f => (decimal?)f.TotalValue) ?? 0m;
    }
}
