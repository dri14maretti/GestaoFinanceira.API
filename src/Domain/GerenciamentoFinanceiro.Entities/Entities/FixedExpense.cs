namespace GerenciamentoFinanceiro.Entities;

public class FixedExpense
{
    public Guid Id { get; private set; }
    public decimal Value { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid CreditCardId { get; private set; }

    private FixedExpense() { }

    public FixedExpense(decimal value, string description, Guid creditCardId)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Id = Guid.NewGuid();
        Value = value;
        Description = description.Trim();
        CreditCardId = creditCardId;
    }

    public void Update(decimal value, string description, Guid creditCardId)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Value = value;
        Description = description.Trim();
        CreditCardId = creditCardId;
    }
}
