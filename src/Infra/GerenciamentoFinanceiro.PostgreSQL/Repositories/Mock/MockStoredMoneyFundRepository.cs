using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockStoredMoneyFundRepository : IStoredMoneyFundRepository
{
    private static readonly List<StoredMoneyFund> _data;

    static MockStoredMoneyFundRepository()
    {
        _data = new List<StoredMoneyFund>();
        Seed(MockSeedData.FundEmergencia, 15000.00m, "Reserva de Emergência");
        Seed(MockSeedData.FundViagem, 3200.00m, "Viagem");
        Seed(MockSeedData.FundCarro, 8500.00m, "Carro Novo");
    }

    private static void Seed(Guid id, decimal totalValue, string description)
    {
        var fund = new StoredMoneyFund(totalValue, description);
        typeof(StoredMoneyFund).GetProperty(nameof(StoredMoneyFund.Id))!.SetValue(fund, id);
        _data.Add(fund);
    }

    public Task<IEnumerable<StoredMoneyFund>> GetAllAsync() => Task.FromResult<IEnumerable<StoredMoneyFund>>(_data.ToList());
    public Task<StoredMoneyFund?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(f => f.Id == id));
    public Task<StoredMoneyFund> AddAsync(StoredMoneyFund entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<StoredMoneyFund?> UpdateAsync(StoredMoneyFund entity) { var i = _data.FindIndex(f => f.Id == entity.Id); if (i < 0) return Task.FromResult<StoredMoneyFund?>(null); _data[i] = entity; return Task.FromResult<StoredMoneyFund?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(f => f.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
