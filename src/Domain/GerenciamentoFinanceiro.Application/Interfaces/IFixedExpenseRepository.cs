using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IFixedExpenseRepository
{
    Task<IEnumerable<FixedExpense>> GetAllAsync();
    Task<FixedExpense?> GetByIdAsync(Guid id);
    Task<IEnumerable<FixedExpense>> GetByCreditCardIdAsync(Guid creditCardId);
    Task<FixedExpense> AddAsync(FixedExpense entity);
    Task<FixedExpense?> UpdateAsync(FixedExpense entity);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetTotalAsync();
}
