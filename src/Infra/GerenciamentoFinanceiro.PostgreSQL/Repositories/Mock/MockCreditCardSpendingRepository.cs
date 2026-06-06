using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockCreditCardSpendingRepository : ICreditCardSpendingRepository
{
    private static readonly List<CreditCardSpending> _data;

    static MockCreditCardSpendingRepository()
    {
        _data = new List<CreditCardSpending>();
        var s = MockSeedData.CatAlimentacao;
        // Jan-May 2026 spendings across categories and cards
        Add(89.90m, MockSeedData.CatAlimentacao, MockSeedData.CcNubank, new DateTime(2026, 1, 5), "Supermercado Extra");
        Add(250.00m, MockSeedData.CatAlimentacao, MockSeedData.CcNubank, new DateTime(2026, 1, 18), "iFood Mensal");
        Add(45.00m, MockSeedData.CatTransporte, MockSeedData.CcNubank, new DateTime(2026, 2, 3), "Uber");
        Add(320.00m, MockSeedData.CatLazer, MockSeedData.CcInter, new DateTime(2026, 2, 14), "Ingresso show");
        Add(150.00m, MockSeedData.CatSaude, MockSeedData.CcItau, new DateTime(2026, 2, 20), "Farmácia");
        Add(1200.00m, MockSeedData.CatMoradia, MockSeedData.CcNubank, new DateTime(2026, 3, 1), "Material construção");
        Add(78.50m, MockSeedData.CatAlimentacao, MockSeedData.CcInter, new DateTime(2026, 3, 10), "Padaria");
        Add(430.00m, MockSeedData.CatEducacao, MockSeedData.CcItau, new DateTime(2026, 3, 15), "Curso Udemy");
        Add(95.00m, MockSeedData.CatTransporte, MockSeedData.CcNubank, new DateTime(2026, 4, 2), "Combustível");
        Add(200.00m, MockSeedData.CatLazer, MockSeedData.CcNubank, new DateTime(2026, 4, 12), "Restaurante");
        Add(180.00m, MockSeedData.CatAlimentacao, MockSeedData.CcInter, new DateTime(2026, 4, 20), "Supermercado BH");
        Add(65.00m, MockSeedData.CatSaude, MockSeedData.CcItau, new DateTime(2026, 5, 1), "Consulta médica");
        Add(340.00m, MockSeedData.CatAlimentacao, MockSeedData.CcNubank, new DateTime(2026, 5, 5), "Mercado semanal");
        Add(120.00m, MockSeedData.CatTransporte, MockSeedData.CcInter, new DateTime(2026, 5, 8), "Manutenção carro");
        Add(55.00m, MockSeedData.CatLazer, MockSeedData.CcNubank, new DateTime(2026, 5, 9), "Cinema");
    }

    private static void Add(decimal value, Guid categoryId, Guid creditCardId, DateTime date, string description)
    {
        _data.Add(new CreditCardSpending(value, categoryId, creditCardId, date, description));
    }

    public Task<IEnumerable<CreditCardSpending>> GetAllAsync() => Task.FromResult<IEnumerable<CreditCardSpending>>(_data.ToList());
    public Task<CreditCardSpending?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(s => s.Id == id));
    public Task<IEnumerable<CreditCardSpending>> GetByMonthAsync(int year, int month) => Task.FromResult<IEnumerable<CreditCardSpending>>(_data.Where(s => s.Date.Year == year && s.Date.Month == month).ToList());
    public Task<IEnumerable<CreditCardSpending>> GetByCreditCardIdAsync(Guid creditCardId) => Task.FromResult<IEnumerable<CreditCardSpending>>(_data.Where(s => s.CreditCardId == creditCardId).ToList());
    public Task<CreditCardSpending> AddAsync(CreditCardSpending entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<CreditCardSpending?> UpdateAsync(CreditCardSpending entity) { var i = _data.FindIndex(s => s.Id == entity.Id); if (i < 0) return Task.FromResult<CreditCardSpending?>(null); _data[i] = entity; return Task.FromResult<CreditCardSpending?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(s => s.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
