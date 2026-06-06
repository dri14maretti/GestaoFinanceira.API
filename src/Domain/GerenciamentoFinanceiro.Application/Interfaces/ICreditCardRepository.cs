using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ICreditCardRepository
{
    Task<IEnumerable<CreditCard>> GetAllAsync();
    Task<CreditCard?> GetByIdAsync(Guid id);
    Task<CreditCard> AddAsync(CreditCard entity);
    Task<CreditCard?> UpdateAsync(CreditCard entity);
    Task<bool> DeleteAsync(Guid id);
}
