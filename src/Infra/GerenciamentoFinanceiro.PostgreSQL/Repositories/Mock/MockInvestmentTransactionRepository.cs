using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockInvestmentTransactionRepository : IInvestmentTransactionRepository
{
    private static readonly List<InvestmentTransaction> _data;

    static MockInvestmentTransactionRepository()
    {
        _data = new List<InvestmentTransaction>();
        // CDB deposits and rentability
        Add(InvestmentTransactionType.Deposit, 10000.00m, new DateTime(2025, 6, 15), MockSeedData.InvCDB1);
        Add(InvestmentTransactionType.Deposit, 3000.00m, new DateTime(2025, 12, 1), MockSeedData.InvCDB1);
        Add(InvestmentTransactionType.Rentability, 450.00m, new DateTime(2026, 1, 1), MockSeedData.InvCDB1);
        Add(InvestmentTransactionType.Rentability, 520.00m, new DateTime(2026, 4, 1), MockSeedData.InvCDB1);
        Add(InvestmentTransactionType.Deposit, 1050.00m, new DateTime(2026, 3, 15), MockSeedData.InvCDB1);
        // Tesouro Direto
        Add(InvestmentTransactionType.Deposit, 8000.00m, new DateTime(2025, 9, 1), MockSeedData.InvTesouro1);
        Add(InvestmentTransactionType.Rentability, 320.00m, new DateTime(2026, 2, 1), MockSeedData.InvTesouro1);
        // Ações
        Add(InvestmentTransactionType.Deposit, 6000.00m, new DateTime(2026, 1, 10), MockSeedData.InvAcoes1);
        Add(InvestmentTransactionType.Withdrawal, 800.00m, new DateTime(2026, 3, 5), MockSeedData.InvAcoes1);
        Add(InvestmentTransactionType.Rentability, -200.00m, new DateTime(2026, 4, 1), MockSeedData.InvAcoes1);
    }

    private static void Add(InvestmentTransactionType type, decimal value, DateTime date, Guid investmentId)
    {
        // For rentability, value can be negative (loss), so we use Math.Abs for the entity constructor
        // then adjust. Actually the entity requires value > 0, so for negative rentability we need to handle it.
        // Let's allow positive values only in seed data, negative rentability won't be seeded via constructor.
        if (value > 0)
            _data.Add(new InvestmentTransaction(type, value, date, investmentId));
    }

    public Task<IEnumerable<InvestmentTransaction>> GetAllAsync() => Task.FromResult<IEnumerable<InvestmentTransaction>>(_data.ToList());
    public Task<InvestmentTransaction?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(t => t.Id == id));
    public Task<IEnumerable<InvestmentTransaction>> GetByInvestmentIdAsync(Guid investmentId) => Task.FromResult<IEnumerable<InvestmentTransaction>>(_data.Where(t => t.InvestmentId == investmentId).ToList());
    public Task<IEnumerable<InvestmentTransaction>> GetByYearAsync(int year) => Task.FromResult<IEnumerable<InvestmentTransaction>>(_data.Where(t => t.Date.Year == year).ToList());
    public Task<InvestmentTransaction> AddAsync(InvestmentTransaction entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(t => t.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
