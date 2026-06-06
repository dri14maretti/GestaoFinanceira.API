using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockCashExpenseRepository : ICashExpenseRepository
{
    private static readonly List<CashExpense> _data;

    static MockCashExpenseRepository()
    {
        _data = new List<CashExpense>();
        Add(1500.00m, MockSeedData.CatMoradia, "Nubank", new DateTime(2026, 1, 5), "Aluguel via PIX");
        Add(200.00m, MockSeedData.CatAlimentacao, "Nubank", new DateTime(2026, 1, 10), "Feira livre");
        Add(350.00m, MockSeedData.CatSaude, "Inter", new DateTime(2026, 2, 3), "Plano de saúde - Boleto");
        Add(80.00m, MockSeedData.CatTransporte, "Bradesco", new DateTime(2026, 2, 15), "Estacionamento PIX");
        Add(1500.00m, MockSeedData.CatMoradia, "Nubank", new DateTime(2026, 3, 5), "Aluguel via PIX");
        Add(450.00m, MockSeedData.CatAlimentacao, "Inter", new DateTime(2026, 3, 12), "Compras do mês");
        Add(120.00m, MockSeedData.CatLazer, "Bradesco", new DateTime(2026, 3, 20), "Assinatura anual");
        Add(1500.00m, MockSeedData.CatMoradia, "Nubank", new DateTime(2026, 4, 5), "Aluguel via PIX");
        Add(280.00m, MockSeedData.CatAlimentacao, "Inter", new DateTime(2026, 4, 10), "Supermercado PIX");
        Add(350.00m, MockSeedData.CatSaude, "Inter", new DateTime(2026, 4, 3), "Plano de saúde - Boleto");
        Add(1500.00m, MockSeedData.CatMoradia, "Nubank", new DateTime(2026, 5, 5), "Aluguel via PIX");
        Add(190.00m, MockSeedData.CatAlimentacao, "Bradesco", new DateTime(2026, 5, 8), "Feira orgânica PIX");
    }

    private static void Add(decimal value, Guid categoryId, string bank, DateTime date, string description)
    {
        _data.Add(new CashExpense(value, categoryId, bank, date, description));
    }

    public Task<IEnumerable<CashExpense>> GetAllAsync() => Task.FromResult<IEnumerable<CashExpense>>(_data.ToList());
    public Task<CashExpense?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(e => e.Id == id));
    public Task<IEnumerable<CashExpense>> GetByMonthAsync(int year, int month) => Task.FromResult<IEnumerable<CashExpense>>(_data.Where(e => e.Date.Year == year && e.Date.Month == month).ToList());
    public Task<IEnumerable<CashExpense>> GetByBankAsync(string bank) => Task.FromResult<IEnumerable<CashExpense>>(_data.Where(e => e.Bank.Equals(bank, StringComparison.OrdinalIgnoreCase)).ToList());
    public Task<CashExpense> AddAsync(CashExpense entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<CashExpense?> UpdateAsync(CashExpense entity) { var i = _data.FindIndex(e => e.Id == entity.Id); if (i < 0) return Task.FromResult<CashExpense?>(null); _data[i] = entity; return Task.FromResult<CashExpense?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(e => e.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
