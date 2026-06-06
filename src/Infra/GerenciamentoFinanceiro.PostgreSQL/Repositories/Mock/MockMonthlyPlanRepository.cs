using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockMonthlyPlanRepository : IMonthlyPlanRepository
{
    private static readonly List<MonthlyPlan> _data;

    static MockMonthlyPlanRepository()
    {
        _data = new List<MonthlyPlan>();
        Seed(MockSeedData.PlanMar2026, new DateTime(2026, 3, 1), 8500.00m);
        Seed(MockSeedData.PlanAbr2026, new DateTime(2026, 4, 1), 8500.00m);
        Seed(MockSeedData.PlanMai2026, new DateTime(2026, 5, 1), 8500.00m);
    }

    private static void Seed(Guid id, DateTime month, decimal income)
    {
        var plan = new MonthlyPlan(month, income);
        typeof(MonthlyPlan).GetProperty(nameof(MonthlyPlan.Id))!.SetValue(plan, id);
        _data.Add(plan);
    }

    public Task<IEnumerable<MonthlyPlan>> GetAllAsync() => Task.FromResult<IEnumerable<MonthlyPlan>>(_data.ToList());
    public Task<MonthlyPlan?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(p => p.Id == id));
    public Task<MonthlyPlan?> GetByMonthAsync(int year, int month) => Task.FromResult(_data.FirstOrDefault(p => p.Month.Year == year && p.Month.Month == month));
    public Task<MonthlyPlan> AddAsync(MonthlyPlan entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<MonthlyPlan?> UpdateAsync(MonthlyPlan entity) { var i = _data.FindIndex(p => p.Id == entity.Id); if (i < 0) return Task.FromResult<MonthlyPlan?>(null); _data[i] = entity; return Task.FromResult<MonthlyPlan?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(p => p.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
