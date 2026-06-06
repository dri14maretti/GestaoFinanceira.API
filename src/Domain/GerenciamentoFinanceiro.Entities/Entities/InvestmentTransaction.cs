using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Entities;

public class InvestmentTransaction
{
    public Guid Id { get; private set; }
    public InvestmentTransactionType Type { get; private set; }
    public decimal Value { get; private set; }
    public DateTime Date { get; private set; }
    public Guid InvestmentId { get; private set; }

    private InvestmentTransaction() { }

    public InvestmentTransaction(InvestmentTransactionType type, decimal value, DateTime date, Guid investmentId)
    {
        if (value <= 0)
            throw new ArgumentException("Value must be greater than zero.", nameof(value));
        if (investmentId == Guid.Empty)
            throw new ArgumentException("InvestmentId is required.", nameof(investmentId));

        Id = Guid.NewGuid();
        Type = type;
        Value = value;
        Date = date;
        InvestmentId = investmentId;
    }
}
