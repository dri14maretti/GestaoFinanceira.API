using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockInvestmentRepository : IInvestmentRepository
{
    private static readonly List<Investment> _data;

    static MockInvestmentRepository()
    {
        _data = new List<Investment>();
        Seed(MockSeedData.InvCDB1, 15000.00m, new DateTime(2025, 6, 15), MockSeedData.InvCatCDB);
        Seed(MockSeedData.InvTesouro1, 8000.00m, new DateTime(2025, 9, 1), MockSeedData.InvCatTesouro);
        Seed(MockSeedData.InvAcoes1, 5200.00m, new DateTime(2026, 1, 10), MockSeedData.InvCatAcoes);
    }

    private static void Seed(Guid id, decimal value, DateTime date, Guid categoryId)
    {
        var inv = new Investment(value, date, categoryId);
        typeof(Investment).GetProperty(nameof(Investment.Id))!.SetValue(inv, id);
        _data.Add(inv);
    }

    public Task<IEnumerable<Investment>> GetAllAsync() => Task.FromResult<IEnumerable<Investment>>(_data.ToList());
    public Task<Investment?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(i => i.Id == id));
    public Task<Investment> AddAsync(Investment entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<Investment?> UpdateAsync(Investment entity) { var i = _data.FindIndex(x => x.Id == entity.Id); if (i < 0) return Task.FromResult<Investment?>(null); _data[i] = entity; return Task.FromResult<Investment?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(i => i.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
