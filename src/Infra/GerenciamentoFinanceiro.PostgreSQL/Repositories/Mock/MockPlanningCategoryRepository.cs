using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockPlanningCategoryRepository : IPlanningCategoryRepository
{
    private static readonly List<PlanningCategory> _data;

    static MockPlanningCategoryRepository()
    {
        _data = new List<PlanningCategory>();
        Seed(MockSeedData.PlanCatMoradia, "Moradia");
        Seed(MockSeedData.PlanCatAlimentacao, "Alimentação");
        Seed(MockSeedData.PlanCatTransporte, "Transporte");
        Seed(MockSeedData.PlanCatLazer, "Lazer");
        Seed(MockSeedData.PlanCatInvestimentos, "Investimentos");
        Seed(MockSeedData.PlanCatEmergencia, "Emergência");
    }

    private static void Seed(Guid id, string name)
    {
        var cat = new PlanningCategory(name);
        typeof(PlanningCategory).GetProperty(nameof(PlanningCategory.Id))!.SetValue(cat, id);
        _data.Add(cat);
    }

    public Task<IEnumerable<PlanningCategory>> GetAllAsync() => Task.FromResult<IEnumerable<PlanningCategory>>(_data.ToList());
    public Task<PlanningCategory?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(c => c.Id == id));
    public Task<PlanningCategory> AddAsync(PlanningCategory entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<PlanningCategory?> UpdateAsync(PlanningCategory entity) { var i = _data.FindIndex(c => c.Id == entity.Id); if (i < 0) return Task.FromResult<PlanningCategory?>(null); _data[i] = entity; return Task.FromResult<PlanningCategory?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(c => c.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
