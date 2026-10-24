using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class InvestmentTransactionRepository : IInvestmentTransactionRepository
{
    private readonly FinanceDbContext _context;

    public InvestmentTransactionRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvestmentTransaction>> GetAllAsync()
    {
        return await _context.InvestmentTransactions.AsNoTracking().ToListAsync();
    }

    public async Task<InvestmentTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.InvestmentTransactions.FindAsync(id);
    }

    public async Task<IEnumerable<InvestmentTransaction>> GetByInvestmentIdAsync(Guid investmentId)
    {
        return await _context.InvestmentTransactions
            .AsNoTracking()
            .Where(t => t.InvestmentId == investmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<InvestmentTransaction>> GetByYearAsync(int year)
    {
        var startOfYear = new DateTime(year, 1, 1);
        var endOfYear = new DateTime(year + 1, 1, 1);

        return await _context.InvestmentTransactions
            .AsNoTracking()
            .Where(t => t.Date >= startOfYear && t.Date < endOfYear)
            .ToListAsync();
    }

    public async Task<InvestmentTransaction> AddAsync(InvestmentTransaction entity)
    {
        await _context.InvestmentTransactions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.InvestmentTransactions.FindAsync(id);
        if (entity == null) return false;

        _context.InvestmentTransactions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetRentabilityTotalByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        return await _context.InvestmentTransactions
            .Where(t => t.Date >= startOfMonth && t.Date < endOfMonth && t.Type == InvestmentTransactionType.Rentability)
            .SumAsync(t => (decimal?)t.Value) ?? 0m;
    }

    public async Task<IEnumerable<InvestmentTransaction>> GetFromDateAsync(DateTime fromDate)
    {
        return await _context.InvestmentTransactions
            .AsNoTracking()
            .Where(t => t.Date >= fromDate)
            .ToListAsync();
    }
}
