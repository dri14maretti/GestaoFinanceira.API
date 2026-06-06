using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockFixedExpenseRepository : IFixedExpenseRepository
{
    private static readonly List<FixedExpense> _data;

    static MockFixedExpenseRepository()
    {
        _data = new List<FixedExpense>();
        Add(39.90m, "Netflix", MockSeedData.CcNubank);
        Add(21.90m, "Spotify", MockSeedData.CcNubank);
        Add(129.90m, "Academia Smart Fit", MockSeedData.CcInter);
        Add(14.90m, "iCloud 200GB", MockSeedData.CcItau);
    }

    private static void Add(decimal value, string description, Guid creditCardId)
    {
        _data.Add(new FixedExpense(value, description, creditCardId));
    }

    public Task<IEnumerable<FixedExpense>> GetAllAsync() => Task.FromResult<IEnumerable<FixedExpense>>(_data.ToList());
    public Task<FixedExpense?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(f => f.Id == id));
    public Task<IEnumerable<FixedExpense>> GetByCreditCardIdAsync(Guid creditCardId) => Task.FromResult<IEnumerable<FixedExpense>>(_data.Where(f => f.CreditCardId == creditCardId).ToList());
    public Task<FixedExpense> AddAsync(FixedExpense entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<FixedExpense?> UpdateAsync(FixedExpense entity) { var i = _data.FindIndex(f => f.Id == entity.Id); if (i < 0) return Task.FromResult<FixedExpense?>(null); _data[i] = entity; return Task.FromResult<FixedExpense?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(f => f.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
