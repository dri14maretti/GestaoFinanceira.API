using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IInvestmentCategoryRepository
{
    Task<IEnumerable<InvestmentCategory>> GetAllAsync();
    Task<InvestmentCategory?> GetByIdAsync(Guid id);
    Task<InvestmentCategory> AddAsync(InvestmentCategory entity);
    Task<InvestmentCategory?> UpdateAsync(InvestmentCategory entity);
    Task<bool> DeleteAsync(Guid id);
}
