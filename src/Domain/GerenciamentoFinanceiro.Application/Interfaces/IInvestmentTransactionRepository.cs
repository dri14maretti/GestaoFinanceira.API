using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IInvestmentTransactionRepository
{
    Task<IEnumerable<InvestmentTransaction>> GetAllAsync();
    Task<InvestmentTransaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<InvestmentTransaction>> GetByInvestmentIdAsync(Guid investmentId);
    Task<IEnumerable<InvestmentTransaction>> GetByYearAsync(int year);
    Task<InvestmentTransaction> AddAsync(InvestmentTransaction entity);
    Task<bool> DeleteAsync(Guid id);
}
