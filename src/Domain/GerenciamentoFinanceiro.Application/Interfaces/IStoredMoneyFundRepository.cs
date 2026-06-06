using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IStoredMoneyFundRepository
{
    Task<IEnumerable<StoredMoneyFund>> GetAllAsync();
    Task<StoredMoneyFund?> GetByIdAsync(Guid id);
    Task<StoredMoneyFund> AddAsync(StoredMoneyFund entity);
    Task<StoredMoneyFund?> UpdateAsync(StoredMoneyFund entity);
    Task<bool> DeleteAsync(Guid id);
}
