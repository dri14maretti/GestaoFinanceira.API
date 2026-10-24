using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ICashExpenseRepository
{
    Task<IEnumerable<CashExpense>> GetAllAsync();
    Task<CashExpense?> GetByIdAsync(Guid id);
    Task<IEnumerable<CashExpense>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<CashExpense>> GetByBankAsync(string bank);
    Task<CashExpense> AddAsync(CashExpense entity);
    Task<CashExpense?> UpdateAsync(CashExpense entity);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetTotalByMonthAsync(int year, int month);
    Task<Dictionary<Guid, decimal>> GetTotalsByCategoryAsync(int year, int month);
    Task<Dictionary<string, decimal>> GetBalancesByBankAsync();
}
