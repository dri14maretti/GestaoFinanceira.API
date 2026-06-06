using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IPlanningCategoryRepository
{
    Task<IEnumerable<PlanningCategory>> GetAllAsync();
    Task<PlanningCategory?> GetByIdAsync(Guid id);
    Task<PlanningCategory> AddAsync(PlanningCategory entity);
    Task<PlanningCategory?> UpdateAsync(PlanningCategory entity);
    Task<bool> DeleteAsync(Guid id);
}
