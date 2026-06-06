namespace GerenciamentoFinanceiro.Entities;

public class PlanAllocation
{
    public Guid Id { get; private set; }
    public Guid MonthlyPlanId { get; private set; }
    public Guid PlanningCategoryId { get; private set; }
    public decimal AllocatedValue { get; private set; }

    private PlanAllocation() { }

    public PlanAllocation(Guid monthlyPlanId, Guid planningCategoryId, decimal allocatedValue)
    {
        if (monthlyPlanId == Guid.Empty)
            throw new ArgumentException("MonthlyPlanId is required.", nameof(monthlyPlanId));
        if (planningCategoryId == Guid.Empty)
            throw new ArgumentException("PlanningCategoryId is required.", nameof(planningCategoryId));
        if (allocatedValue < 0)
            throw new ArgumentException("Allocated value cannot be negative.", nameof(allocatedValue));

        Id = Guid.NewGuid();
        MonthlyPlanId = monthlyPlanId;
        PlanningCategoryId = planningCategoryId;
        AllocatedValue = allocatedValue;
    }

    public void UpdateAllocatedValue(decimal allocatedValue)
    {
        if (allocatedValue < 0)
            throw new ArgumentException("Allocated value cannot be negative.", nameof(allocatedValue));

        AllocatedValue = allocatedValue;
    }
}
