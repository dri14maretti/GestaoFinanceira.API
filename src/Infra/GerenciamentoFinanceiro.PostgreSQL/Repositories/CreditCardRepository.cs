using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly FinanceDbContext _context;

    public CreditCardRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CreditCard>> GetAllAsync()
    {
        return await _context.CreditCards.AsNoTracking().ToListAsync();
    }

    public async Task<CreditCard?> GetByIdAsync(Guid id)
    {
        return await _context.CreditCards.FindAsync(id);
    }

    public async Task<CreditCard> AddAsync(CreditCard entity)
    {
        await _context.CreditCards.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<CreditCard?> UpdateAsync(CreditCard entity)
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var existing = await _context.CreditCards.FindAsync(entity.Id);
            if (existing == null) return null;
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.CreditCards.FindAsync(id);
        if (entity == null) return false;

        _context.CreditCards.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
