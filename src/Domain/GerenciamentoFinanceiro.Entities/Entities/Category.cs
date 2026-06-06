using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public CategoryType Type { get; private set; }

    private Category() { }

    public Category(string name, CategoryType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Type = type;
    }

    public void Update(string name, CategoryType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        Name = name.Trim();
        Type = type;
    }
}
