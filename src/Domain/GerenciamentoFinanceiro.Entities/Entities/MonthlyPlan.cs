namespace GerenciamentoFinanceiro.Entities;

public class MonthlyPlan
{
    public Guid Id { get; private set; }
    public DateTime Month { get; private set; }
    public decimal Income { get; private set; }

    private MonthlyPlan() { }

    public MonthlyPlan(DateTime month, decimal income)
    {
        if (income < 0)
            throw new ArgumentException("Income cannot be negative.", nameof(income));

        Id = Guid.NewGuid();
        Month = new DateTime(month.Year, month.Month, 1); // Normalize to first of month
        Income = income;
    }

    public void UpdateIncome(decimal income)
    {
        if (income < 0)
            throw new ArgumentException("Income cannot be negative.", nameof(income));

        Income = income;
    }
}
