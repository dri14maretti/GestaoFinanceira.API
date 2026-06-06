using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockFundTransactionRepository : IFundTransactionRepository
{
    private static readonly List<FundTransaction> _data;

    static MockFundTransactionRepository()
    {
        _data = new List<FundTransaction>();
        Add(FundTransactionType.Add, 5000.00m, MockSeedData.FundEmergencia, new DateTime(2025, 8, 1));
        Add(FundTransactionType.Add, 5000.00m, MockSeedData.FundEmergencia, new DateTime(2025, 11, 1));
        Add(FundTransactionType.Add, 5000.00m, MockSeedData.FundEmergencia, new DateTime(2026, 2, 1));
        Add(FundTransactionType.Add, 1500.00m, MockSeedData.FundViagem, new DateTime(2026, 1, 15));
        Add(FundTransactionType.Add, 1700.00m, MockSeedData.FundViagem, new DateTime(2026, 3, 15));
        Add(FundTransactionType.Add, 3000.00m, MockSeedData.FundCarro, new DateTime(2025, 10, 1));
        Add(FundTransactionType.Add, 3500.00m, MockSeedData.FundCarro, new DateTime(2026, 1, 1));
        Add(FundTransactionType.Add, 2000.00m, MockSeedData.FundCarro, new DateTime(2026, 4, 1));
    }

    private static void Add(FundTransactionType type, decimal value, Guid fundId, DateTime date)
    {
        _data.Add(new FundTransaction(type, value, fundId, date));
    }

    public Task<IEnumerable<FundTransaction>> GetAllAsync() => Task.FromResult<IEnumerable<FundTransaction>>(_data.ToList());
    public Task<FundTransaction?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(t => t.Id == id));
    public Task<IEnumerable<FundTransaction>> GetByFundIdAsync(Guid fundId) => Task.FromResult<IEnumerable<FundTransaction>>(_data.Where(t => t.FundId == fundId).ToList());
    public Task<FundTransaction> AddAsync(FundTransaction entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(t => t.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
