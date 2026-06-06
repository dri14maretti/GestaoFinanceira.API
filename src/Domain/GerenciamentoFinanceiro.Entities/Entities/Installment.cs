namespace GerenciamentoFinanceiro.Entities;

public class Installment
{
    public Guid Id { get; private set; }
    public decimal Value { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid CreditCardId { get; private set; }
    public DateTime Month { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int TotalInstallments { get; private set; }
    public int CurrentInstallment { get; private set; }

    private Installment() { }

    public Installment(decimal value, Guid categoryId, Guid creditCardId, DateTime month, string description, int totalInstallments, int currentInstallment)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (totalInstallments < 1)
            throw new ArgumentException("Total installments must be at least 1.", nameof(totalInstallments));
        if (currentInstallment < 1 || currentInstallment > totalInstallments)
            throw new ArgumentOutOfRangeException(nameof(currentInstallment), "Current installment must be between 1 and total installments.");

        Id = Guid.NewGuid();
        Value = value;
        CategoryId = categoryId;
        CreditCardId = creditCardId;
        Month = new DateTime(month.Year, month.Month, 1); // Normalize to first day of month
        Description = description?.Trim() ?? string.Empty;
        TotalInstallments = totalInstallments;
        CurrentInstallment = currentInstallment;
    }

    public void Update(decimal value, Guid categoryId, Guid creditCardId, DateTime month, string description, int totalInstallments, int currentInstallment)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (totalInstallments < 1)
            throw new ArgumentException("Total installments must be at least 1.", nameof(totalInstallments));
        if (currentInstallment < 1 || currentInstallment > totalInstallments)
            throw new ArgumentOutOfRangeException(nameof(currentInstallment));

        Value = value;
        CategoryId = categoryId;
        CreditCardId = creditCardId;
        Month = new DateTime(month.Year, month.Month, 1);
        Description = description?.Trim() ?? string.Empty;
        TotalInstallments = totalInstallments;
        CurrentInstallment = currentInstallment;
    }
}
