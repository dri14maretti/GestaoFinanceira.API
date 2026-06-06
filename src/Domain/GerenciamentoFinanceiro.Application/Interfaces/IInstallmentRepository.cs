using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface IInstallmentRepository
{
    Task<IEnumerable<Installment>> GetAllAsync();
    Task<Installment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Installment>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<Installment>> GetByCreditCardIdAsync(Guid creditCardId);
    Task<Installment> AddAsync(Installment entity);
    Task<Installment?> UpdateAsync(Installment entity);
    Task<bool> DeleteAsync(Guid id);
}
