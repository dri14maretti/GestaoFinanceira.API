using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockCreditCardRepository : ICreditCardRepository
{
    private static readonly List<CreditCard> _data;

    static MockCreditCardRepository()
    {
        _data = new List<CreditCard>();
        Seed(MockSeedData.CcNubank, "Nubank", 15, 22);
        Seed(MockSeedData.CcInter, "Inter", 10, 17);
        Seed(MockSeedData.CcItau, "Itaú", 5, 12);
    }

    private static void Seed(Guid id, string bankName, int closingDate, int dueDate)
    {
        var card = new CreditCard(bankName, closingDate, dueDate);
        typeof(CreditCard).GetProperty(nameof(CreditCard.Id))!.SetValue(card, id);
        _data.Add(card);
    }

    public Task<IEnumerable<CreditCard>> GetAllAsync() => Task.FromResult<IEnumerable<CreditCard>>(_data.ToList());
    public Task<CreditCard?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(c => c.Id == id));
    public Task<CreditCard> AddAsync(CreditCard entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<CreditCard?> UpdateAsync(CreditCard entity) { var i = _data.FindIndex(c => c.Id == entity.Id); if (i < 0) return Task.FromResult<CreditCard?>(null); _data[i] = entity; return Task.FromResult<CreditCard?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(c => c.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
