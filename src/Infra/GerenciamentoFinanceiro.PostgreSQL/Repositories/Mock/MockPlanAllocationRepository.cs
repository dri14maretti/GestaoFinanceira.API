using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockPlanAllocationRepository : IPlanAllocationRepository
{
    private static readonly List<PlanAllocation> _data;

    static MockPlanAllocationRepository()
    {
        _data = new List<PlanAllocation>();
        // March 2026 allocations
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatMoradia, 2500.00m);
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatAlimentacao, 1500.00m);
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatTransporte, 800.00m);
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatLazer, 500.00m);
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatInvestimentos, 2000.00m);
        Add(MockSeedData.PlanMar2026, MockSeedData.PlanCatEmergencia, 1000.00m);
        // April 2026 allocations
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatMoradia, 2500.00m);
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatAlimentacao, 1500.00m);
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatTransporte, 800.00m);
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatLazer, 600.00m);
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatInvestimentos, 2000.00m);
        Add(MockSeedData.PlanAbr2026, MockSeedData.PlanCatEmergencia, 1000.00m);
        // May 2026 allocations
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatMoradia, 2500.00m);
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatAlimentacao, 1600.00m);
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatTransporte, 700.00m);
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatLazer, 500.00m);
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatInvestimentos, 2200.00m);
        Add(MockSeedData.PlanMai2026, MockSeedData.PlanCatEmergencia, 800.00m);
    }

    private static void Add(Guid planId, Guid categoryId, decimal value)
    {
        _data.Add(new PlanAllocation(planId, categoryId, value));
    }

    public Task<IEnumerable<PlanAllocation>> GetAllAsync() => Task.FromResult<IEnumerable<PlanAllocation>>(_data.ToList());
    public Task<PlanAllocation?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(a => a.Id == id));
    public Task<IEnumerable<PlanAllocation>> GetByPlanIdAsync(Guid monthlyPlanId) => Task.FromResult<IEnumerable<PlanAllocation>>(_data.Where(a => a.MonthlyPlanId == monthlyPlanId).ToList());
    public Task<PlanAllocation> AddAsync(PlanAllocation entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<PlanAllocation?> UpdateAsync(PlanAllocation entity) { var i = _data.FindIndex(a => a.Id == entity.Id); if (i < 0) return Task.FromResult<PlanAllocation?>(null); _data[i] = entity; return Task.FromResult<PlanAllocation?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(a => a.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
