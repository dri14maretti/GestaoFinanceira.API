using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Dashboard;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private static (int Year, int Month) ResolveMonth(string? month, int? year, int? monthNumber)
    {
        if (!string.IsNullOrWhiteSpace(month) &&
            DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return (parsedDate.Year, parsedDate.Month);
        }
        return (year ?? DateTime.Now.Year, monthNumber ?? DateTime.Now.Month);
    }

    [HttpGet("kpis")]
    public async Task<ActionResult<DashboardKpisDto>> GetKpis(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber)
    {
        var (y, m) = ResolveMonth(month, year, monthNumber);
        var kpis = await _dashboardService.GetDashboardKpisAsync(y, m);
        return Ok(kpis);
    }

    [HttpGet("expenses-by-category")]
    public async Task<ActionResult<IEnumerable<ExpensesByCategoryDto>>> GetExpensesByCategory(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber)
    {
        var (y, m) = ResolveMonth(month, year, monthNumber);
        var breakdown = await _dashboardService.GetExpensesByCategoryAsync(y, m);
        return Ok(breakdown);
    }

    [HttpGet("expenses-by-type")]
    public async Task<ActionResult<ExpensesByTypeDto>> GetExpensesByType(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber)
    {
        var (y, m) = ResolveMonth(month, year, monthNumber);
        var breakdown = await _dashboardService.GetExpensesByTypeAsync(y, m);
        return Ok(breakdown);
    }
}
