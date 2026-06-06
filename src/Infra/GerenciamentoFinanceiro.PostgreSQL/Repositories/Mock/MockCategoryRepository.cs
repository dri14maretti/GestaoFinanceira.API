using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

public class MockCategoryRepository : ICategoryRepository
{
    private static readonly List<Category> _data;

    static MockCategoryRepository()
    {
        _data = new List<Category>();
        Seed(MockSeedData.CatAlimentacao, "Alimentação", CategoryType.Expense);
        Seed(MockSeedData.CatTransporte, "Transporte", CategoryType.Expense);
        Seed(MockSeedData.CatLazer, "Lazer", CategoryType.Expense);
        Seed(MockSeedData.CatSaude, "Saúde", CategoryType.Expense);
        Seed(MockSeedData.CatEducacao, "Educação", CategoryType.Expense);
        Seed(MockSeedData.CatMoradia, "Moradia", CategoryType.Expense);
    }

    private static void Seed(Guid id, string name, CategoryType type)
    {
        var cat = new Category(name, type);
        typeof(Category).GetProperty(nameof(Category.Id))!.SetValue(cat, id);
        _data.Add(cat);
    }

    public Task<IEnumerable<Category>> GetAllAsync() => Task.FromResult<IEnumerable<Category>>(_data.ToList());
    public Task<Category?> GetByIdAsync(Guid id) => Task.FromResult(_data.FirstOrDefault(c => c.Id == id));
    public Task<IEnumerable<Category>> GetByTypeAsync(CategoryType type) => Task.FromResult<IEnumerable<Category>>(_data.Where(c => c.Type == type).ToList());
    public Task<Category> AddAsync(Category entity) { _data.Add(entity); return Task.FromResult(entity); }
    public Task<Category?> UpdateAsync(Category entity) { var i = _data.FindIndex(c => c.Id == entity.Id); if (i < 0) return Task.FromResult<Category?>(null); _data[i] = entity; return Task.FromResult<Category?>(entity); }
    public Task<bool> DeleteAsync(Guid id) { var entity = _data.FirstOrDefault(c => c.Id == id); return Task.FromResult(entity != null && _data.Remove(entity)); }
}
