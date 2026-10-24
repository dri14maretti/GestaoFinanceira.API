using System;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Entities;

public class Category : IUserOwnedEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
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

    public void AssignOwner(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (UserId != Guid.Empty && UserId != userId)
            throw new InvalidOperationException("Entity owner cannot be reassigned.");

        UserId = userId;
    }

    public void Update(string name, CategoryType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.", nameof(name));

        Name = name.Trim();
        Type = type;
    }
}
