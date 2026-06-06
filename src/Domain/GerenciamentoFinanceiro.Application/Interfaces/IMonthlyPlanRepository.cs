using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IMonthlyPlanRepository
{
    Task<IEnumerable<MonthlyPlan>> GetAllAsync();
    Task<MonthlyPlan?> GetByIdAsync(Guid id);
    Task<MonthlyPlan?> GetByMonthAsync(int year, int month);
    Task<MonthlyPlan> AddAsync(MonthlyPlan entity);
    Task<MonthlyPlan?> UpdateAsync(MonthlyPlan entity);
    Task<bool> DeleteAsync(Guid id);
}
