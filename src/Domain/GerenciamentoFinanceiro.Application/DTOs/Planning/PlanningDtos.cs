namespace GerenciamentoFinanceiro.Application.DTOs.Planning;

public record MonthlyPlanDto(
    Guid Id,
    DateTime Month,
    decimal Income,
    List<PlanAllocationDto> Allocations,
    decimal TotalAllocated,
    decimal Surplus
);

public record CreateMonthlyPlanDto(
    DateTime Month,
    decimal Income
);

public record PlanAllocationDto(
    Guid Id,
    Guid MonthlyPlanId,
    Guid PlanningCategoryId,
    string PlanningCategoryName,
    decimal AllocatedValue
);

public record CreatePlanAllocationDto(
    Guid MonthlyPlanId,
    Guid PlanningCategoryId,
    decimal AllocatedValue
);

public record PlanningCategoryDto(
    Guid Id,
    string Name
);

public record CreatePlanningCategoryDto(
    string Name
);
