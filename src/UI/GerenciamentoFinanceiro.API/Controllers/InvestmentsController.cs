using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Dashboard;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentsController : ControllerBase
{
    private readonly InvestmentService _investmentService;
    private readonly DashboardService _dashboardService;

    public InvestmentsController(InvestmentService investmentService, DashboardService dashboardService)
    {
        _investmentService = investmentService;
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvestmentDto>>> GetAll()
    {
        var list = await _investmentService.GetAllInvestmentsAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvestmentDto>> GetById(Guid id)
    {
        var item = await _investmentService.GetInvestmentByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InvestmentDto>> Create([FromBody] CreateInvestmentDto dto)
    {
        var created = await _investmentService.CreateInvestmentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvestmentDto>> Update(Guid id, [FromBody] CreateInvestmentDto dto)
    {
        var updated = await _investmentService.UpdateInvestmentAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _investmentService.DeleteInvestmentAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // --- Transactions ---

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<IEnumerable<InvestmentTransactionDto>>> GetTransactions(Guid id)
    {
        var transactions = await _investmentService.GetTransactionsByInvestmentAsync(id);
        return Ok(transactions);
    }

    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<InvestmentTransactionDto>> CreateTransaction(Guid id, [FromBody] CreateInvestmentTransactionDto dto)
    {
        // Ensure investmentId in route matches body
        var payload = dto with { InvestmentId = id };
        var created = await _investmentService.CreateTransactionAsync(payload);
        return Ok(created);
    }

    [HttpDelete("transactions/{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var deleted = await _investmentService.DeleteTransactionAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // --- Reports & History ---

    [HttpGet("reports/annual")]
    public async Task<ActionResult<AnnualInvestmentReportDto>> GetAnnualReport([FromQuery] int? year)
    {
        var y = year ?? DateTime.Now.Year;
        var report = await _investmentService.GetAnnualReportAsync(y);
        return Ok(report);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<InvestmentHistoryPointDto>>> GetHistory([FromQuery] string? range)
    {
        int months = 6;
        if (!string.IsNullOrWhiteSpace(range))
        {
            if (range.EndsWith("m", StringComparison.OrdinalIgnoreCase) && int.TryParse(range[..^1], out var m))
                months = m;
            else if (range.EndsWith("y", StringComparison.OrdinalIgnoreCase) && int.TryParse(range[..^1], out var y))
                months = y * 12;
        }

        var history = await _dashboardService.GetInvestmentHistoryAsync(months);
        return Ok(history);
    }
}
