using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories;

public class FundTransactionRepository : IFundTransactionRepository
{
    private readonly FinanceDbContext _context;

    public FundTransactionRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FundTransaction>> GetAllAsync()
    {
        return await _context.FundTransactions.AsNoTracking().ToListAsync();
    }

    public async Task<FundTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.FundTransactions.FindAsync(id);
    }

    public async Task<IEnumerable<FundTransaction>> GetByFundIdAsync(Guid fundId)
    {
        return await _context.FundTransactions
            .AsNoTracking()
            .Where(t => t.FundId == fundId)
            .ToListAsync();
    }

    public async Task<FundTransaction> AddAsync(FundTransaction entity)
    {
        await _context.FundTransactions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.FundTransactions.FindAsync(id);
        if (entity == null) return false;

        _context.FundTransactions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetNetChangeByMonthAsync(int year, int month)
    {
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var adds = await _context.FundTransactions
            .Where(t => t.Date >= startOfMonth && t.Date < endOfMonth && t.Type == FundTransactionType.Add)
            .SumAsync(t => (decimal?)t.Value) ?? 0m;

        var withdraws = await _context.FundTransactions
            .Where(t => t.Date >= startOfMonth && t.Date < endOfMonth && t.Type == FundTransactionType.Withdraw)
            .SumAsync(t => (decimal?)t.Value) ?? 0m;

        return adds - withdraws;
    }
}
