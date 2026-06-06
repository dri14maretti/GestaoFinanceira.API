namespace GerenciamentoFinanceiro.Entities;

public class InvestmentCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private InvestmentCategory() { }

    public InvestmentCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
    }
}
