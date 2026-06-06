using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetByTypeAsync(CategoryType type);
    Task<Category> AddAsync(Category entity);
    Task<Category?> UpdateAsync(Category entity);
    Task<bool> DeleteAsync(Guid id);
}
