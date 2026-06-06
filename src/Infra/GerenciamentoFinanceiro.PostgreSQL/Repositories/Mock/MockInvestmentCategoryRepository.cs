using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockInvestmentCategoryRepository : IInvestmentCategoryRepository
{
    private static readonly List<InvestmentCategory> _data;

    static MockInvestmentCategoryRepository()
    {
        _data = new List<InvestmentCategory>();
        Seed(MockSeedData.InvCatCDB, "CDB");
        Seed(MockSeedData.InvCatTesouro, "Tesouro Direto");
        Seed(MockSeedData.InvCatAcoes, "Ações");
        Seed(MockSeedData.InvCatFII, "Fundos Imobiliários");
    }

    private static void Seed(Guid id, string name)
    {
        var cat = new InvestmentCategory(name);
        typeof(InvestmentCategory).GetProperty(nameof(InvestmentCategory.Id))!.SetValue(cat, id);
        _data.Add(cat);
    }

    public Task<IEnumerable<InvestmentCategory>> GetAllAsync() => Task.FromResult<IEnumerable<InvestmentCategory>>(_data.ToList());
    public Task<InvestmentCategory?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(c => c.Id == id));
    public Task<InvestmentCategory> AddAsync(InvestmentCategory entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<InvestmentCategory?> UpdateAsync(InvestmentCategory entity) { var i = _data.FindIndex(c => c.Id == entity.Id); if (i < 0) return Task.FromResult<InvestmentCategory?>(null); _data[i] = entity; return Task.FromResult<InvestmentCategory?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(c => c.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
