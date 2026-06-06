using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IFundTransactionRepository
{
    Task<IEnumerable<FundTransaction>> GetAllAsync();
    Task<FundTransaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<FundTransaction>> GetByFundIdAsync(Guid fundId);
    Task<FundTransaction> AddAsync(FundTransaction entity);
    Task<bool> DeleteAsync(Guid id);
}
