using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IInvestmentRepository
{
    Task<IEnumerable<Investment>> GetAllAsync();
    Task<Investment?> GetByIdAsync(Guid id);
    Task<Investment> AddAsync(Investment entity);
    Task<Investment?> UpdateAsync(Investment entity);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetTotalValueAsync();
}
