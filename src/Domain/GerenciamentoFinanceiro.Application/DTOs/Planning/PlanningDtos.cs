using System.ComponentModel.DataAnnotations;

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
    [property: Required] DateTime Month,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Income must be greater than zero.")] decimal Income
);

public record PlanAllocationDto(
    Guid Id,
    Guid MonthlyPlanId,
    Guid PlanningCategoryId,
    string PlanningCategoryName,
    decimal AllocatedValue
);

public record CreatePlanAllocationDto(
    [property: Required] Guid MonthlyPlanId,
    [property: Required] Guid PlanningCategoryId,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "AllocatedValue must be greater than zero.")] decimal AllocatedValue
);

public record PlanningCategoryDto(
    Guid Id,
    string Name
);

public record CreatePlanningCategoryDto(
    [property: Required, MaxLength(100)] string Name
);
