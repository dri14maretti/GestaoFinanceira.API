using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly PlanningService _planningService;

    public PlansController(PlanningService planningService)
    {
        _planningService = planningService;
    }

    [HttpGet]
    public async Task<ActionResult> GetPlans([FromQuery] string? month, [FromQuery] int? year, [FromQuery] int? monthNumber)
    {
        int? y = year;
        int? m = monthNumber;

        if (!string.IsNullOrWhiteSpace(month) &&
            DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            y = parsedDate.Year;
            m = parsedDate.Month;
        }

        if (y.HasValue && m.HasValue)
        {
            var plan = await _planningService.GetPlanByMonthAsync(y.Value, m.Value);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        var plans = await _planningService.GetAllPlansAsync();
        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MonthlyPlanDto>> GetById(Guid id)
    {
        // Find in all plans
        var plans = await _planningService.GetAllPlansAsync();
        foreach (var p in plans)
        {
            if (p.Id == id) return Ok(p);
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<MonthlyPlanDto>> CreateOrGet([FromBody] CreateMonthlyPlanDto dto)
    {
        var plan = await _planningService.GetOrCreatePlanAsync(dto.Month.Year, dto.Month.Month, dto.Income);
        return Ok(plan);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MonthlyPlanDto>> UpdateIncome(Guid id, [FromBody] decimal income)
    {
        var updated = await _planningService.UpdatePlanIncomeAsync(id, income);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _planningService.DeletePlanAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // --- Allocations ---

    [HttpGet("{id:guid}/allocations")]
    public async Task<ActionResult<IEnumerable<PlanAllocationDto>>> GetAllocations(Guid id)
    {
        var list = await _planningService.GetAllocationsByPlanAsync(id);
        return Ok(list);
    }

    [HttpPost("{id:guid}/allocations")]
    public async Task<ActionResult<PlanAllocationDto>> CreateAllocation(Guid id, [FromBody] CreatePlanAllocationDto dto)
    {
        var payload = dto with { MonthlyPlanId = id };
        var created = await _planningService.CreateAllocationAsync(payload);
        return Ok(created);
    }

    [HttpPut("allocations/{allocationId:guid}")]
    public async Task<ActionResult<PlanAllocationDto>> UpdateAllocation(Guid allocationId, [FromBody] decimal allocatedValue)
    {
        var updated = await _planningService.UpdateAllocationAsync(allocationId, allocatedValue);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("allocations/{allocationId:guid}")]
    public async Task<IActionResult> DeleteAllocation(Guid allocationId)
    {
        var deleted = await _planningService.DeleteAllocationAsync(allocationId);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
