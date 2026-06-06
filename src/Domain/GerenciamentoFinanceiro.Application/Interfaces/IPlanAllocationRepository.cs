using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IPlanAllocationRepository
{
    Task<IEnumerable<PlanAllocation>> GetAllAsync();
    Task<PlanAllocation?> GetByIdAsync(Guid id);
    Task<IEnumerable<PlanAllocation>> GetByPlanIdAsync(Guid monthlyPlanId);
    Task<PlanAllocation> AddAsync(PlanAllocation entity);
    Task<PlanAllocation?> UpdateAsync(PlanAllocation entity);
    Task<bool> DeleteAsync(Guid id);
}
