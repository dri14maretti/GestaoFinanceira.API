using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockInstallmentRepository : IInstallmentRepository
{
    private static readonly List<Installment> _data;

    static MockInstallmentRepository()
    {
        _data = new List<Installment>();
        Add(450.00m, MockSeedData.CatEducacao, MockSeedData.CcNubank, new DateTime(2026, 3, 1), "Curso de inglês", 12, 3);
        Add(250.00m, MockSeedData.CatLazer, MockSeedData.CcInter, new DateTime(2026, 4, 1), "TV 55 polegadas", 10, 7);
        Add(180.00m, MockSeedData.CatSaude, MockSeedData.CcItau, new DateTime(2026, 5, 1), "Óculos de grau", 6, 2);
        Add(800.00m, MockSeedData.CatMoradia, MockSeedData.CcNubank, new DateTime(2026, 5, 1), "Sofá novo", 12, 1);
        Add(120.00m, MockSeedData.CatTransporte, MockSeedData.CcInter, new DateTime(2026, 4, 1), "Pneu carro", 4, 3);
    }

    private static void Add(decimal value, Guid categoryId, Guid creditCardId, DateTime month, string description, int total, int current)
    {
        _data.Add(new Installment(value, categoryId, creditCardId, month, description, total, current));
    }

    public Task<IEnumerable<Installment>> GetAllAsync() => Task.FromResult<IEnumerable<Installment>>(_data.ToList());
    public Task<Installment?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(i => i.Id == id));
    public Task<IEnumerable<Installment>> GetByMonthAsync(int year, int month) => Task.FromResult<IEnumerable<Installment>>(_data.Where(i => i.Month.Year == year && i.Month.Month == month).ToList());
    public Task<IEnumerable<Installment>> GetByCreditCardIdAsync(Guid creditCardId) => Task.FromResult<IEnumerable<Installment>>(_data.Where(i => i.CreditCardId == creditCardId).ToList());
    public Task<Installment> AddAsync(Installment entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<Installment?> UpdateAsync(Installment entity) { var i = _data.FindIndex(x => x.Id == entity.Id); if (i < 0) return Task.FromResult<Installment?>(null); _data[i] = entity; return Task.FromResult<Installment?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(i => i.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
