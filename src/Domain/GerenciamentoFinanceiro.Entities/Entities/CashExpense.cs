namespace GerenciamentoFinanceiro.Entities;

public class CashExpense
{
    public Guid Id { get; private set; }
    public decimal Value { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Bank { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private CashExpense() { }

    public CashExpense(decimal value, Guid categoryId, string bank, DateTime date, string description)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(bank))
            throw new ArgumentException("Bank is required.", nameof(bank));

        Id = Guid.NewGuid();
        Value = value;
        CategoryId = categoryId;
        Bank = bank.Trim();
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }

    public void Update(decimal value, Guid categoryId, string bank, DateTime date, string description)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (string.IsNullOrWhiteSpace(bank))
            throw new ArgumentException("Bank is required.", nameof(bank));

        Value = value;
        CategoryId = categoryId;
        Bank = bank.Trim();
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }
}
