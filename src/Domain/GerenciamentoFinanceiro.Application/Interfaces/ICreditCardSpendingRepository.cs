using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ICreditCardSpendingRepository
{
    Task<IEnumerable<CreditCardSpending>> GetAllAsync();
    Task<CreditCardSpending?> GetByIdAsync(Guid id);
    Task<IEnumerable<CreditCardSpending>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<CreditCardSpending>> GetByCreditCardIdAsync(Guid creditCardId);
    Task<CreditCardSpending> AddAsync(CreditCardSpending entity);
    Task<CreditCardSpending?> UpdateAsync(CreditCardSpending entity);
    Task<bool> DeleteAsync(Guid id);
}
