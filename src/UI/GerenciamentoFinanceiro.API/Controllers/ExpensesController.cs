using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Expenses;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenseService;

    public ExpensesController(ExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private static (int? Year, int? Month) ParseMonthQuery(string? month, int? year, int? monthNumber)
    {
        if (!string.IsNullOrWhiteSpace(month))
        {
            if (DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return (parsedDate.Year, parsedDate.Month);
            }
            if (int.TryParse(month, out var parsedMonth) && parsedMonth >= 1 && parsedMonth <= 12)
            {
                return (year, parsedMonth);
            }
        }
        return (year, monthNumber);
    }

    // ==========================================
    // Credit Card Spendings
    // ==========================================

    [HttpGet("credit-card")]
    public async Task<ActionResult<IEnumerable<CreditCardSpendingDto>>> GetCreditCardSpendings(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber,
        [FromQuery] Guid? cardId,
        [FromQuery] Guid? categoryId)
    {
        var (y, m) = ParseMonthQuery(month, year, monthNumber);
        var list = await _expenseService.GetAllCreditCardSpendingsAsync(y, m, cardId, categoryId);
        return Ok(list);
    }

    [HttpGet("credit-card/{id:guid}")]
    public async Task<ActionResult<CreditCardSpendingDto>> GetCreditCardSpendingById(Guid id)
    {
        var item = await _expenseService.GetCreditCardSpendingByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("credit-card")]
    public async Task<ActionResult<CreditCardSpendingDto>> CreateCreditCardSpending([FromBody] CreateCreditCardSpendingDto dto)
    {
        var created = await _expenseService.CreateCreditCardSpendingAsync(dto);
        return CreatedAtAction(nameof(GetCreditCardSpendingById), new { id = created.Id }, created);
    }

    [HttpPut("credit-card/{id:guid}")]
    public async Task<ActionResult<CreditCardSpendingDto>> UpdateCreditCardSpending(Guid id, [FromBody] CreateCreditCardSpendingDto dto)
    {
        var updated = await _expenseService.UpdateCreditCardSpendingAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("credit-card/{id:guid}")]
    public async Task<IActionResult> DeleteCreditCardSpending(Guid id)
    {
        var deleted = await _expenseService.DeleteCreditCardSpendingAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ==========================================
    // Cash Expenses
    // ==========================================

    [HttpGet("cash")]
    public async Task<ActionResult<IEnumerable<CashExpenseDto>>> GetCashExpenses(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber,
        [FromQuery] string? bank,
        [FromQuery] Guid? categoryId)
    {
        var (y, m) = ParseMonthQuery(month, year, monthNumber);
        var list = await _expenseService.GetAllCashExpensesAsync(y, m, bank, categoryId);
        return Ok(list);
    }

    [HttpGet("cash/balances")]
    public async Task<ActionResult<IEnumerable<BankBalanceDto>>> GetBankBalances()
    {
        var balances = await _expenseService.GetBankBalancesAsync();
        return Ok(balances);
    }

    [HttpGet("cash/{id:guid}")]
    public async Task<ActionResult<CashExpenseDto>> GetCashExpenseById(Guid id)
    {
        var item = await _expenseService.GetCashExpenseByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("cash")]
    public async Task<ActionResult<CashExpenseDto>> CreateCashExpense([FromBody] CreateCashExpenseDto dto)
    {
        var created = await _expenseService.CreateCashExpenseAsync(dto);
        return CreatedAtAction(nameof(GetCashExpenseById), new { id = created.Id }, created);
    }

    [HttpPut("cash/{id:guid}")]
    public async Task<ActionResult<CashExpenseDto>> UpdateCashExpense(Guid id, [FromBody] CreateCashExpenseDto dto)
    {
        var updated = await _expenseService.UpdateCashExpenseAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("cash/{id:guid}")]
    public async Task<IActionResult> DeleteCashExpense(Guid id)
    {
        var deleted = await _expenseService.DeleteCashExpenseAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ==========================================
    // Installments
    // ==========================================

    [HttpGet("installments")]
    public async Task<ActionResult<IEnumerable<InstallmentDto>>> GetInstallments(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber,
        [FromQuery] Guid? cardId)
    {
        var (y, m) = ParseMonthQuery(month, year, monthNumber);
        var list = await _expenseService.GetAllInstallmentsAsync(y, m, cardId);
        return Ok(list);
    }

    [HttpGet("installments/{id:guid}")]
    public async Task<ActionResult<InstallmentDto>> GetInstallmentById(Guid id)
    {
        var item = await _expenseService.GetInstallmentByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("installments")]
    public async Task<ActionResult<InstallmentDto>> CreateInstallment([FromBody] CreateInstallmentDto dto)
    {
        var created = await _expenseService.CreateInstallmentAsync(dto);
        return CreatedAtAction(nameof(GetInstallmentById), new { id = created.Id }, created);
    }

    [HttpPut("installments/{id:guid}")]
    public async Task<ActionResult<InstallmentDto>> UpdateInstallment(Guid id, [FromBody] CreateInstallmentDto dto)
    {
        var updated = await _expenseService.UpdateInstallmentAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("installments/{id:guid}")]
    public async Task<IActionResult> DeleteInstallment(Guid id)
    {
        var deleted = await _expenseService.DeleteInstallmentAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ==========================================
    // Fixed Expenses
    // ==========================================

    [HttpGet("fixed")]
    public async Task<ActionResult<IEnumerable<FixedExpenseDto>>> GetFixedExpenses([FromQuery] Guid? cardId)
    {
        var list = await _expenseService.GetAllFixedExpensesAsync(cardId);
        return Ok(list);
    }

    [HttpGet("fixed/{id:guid}")]
    public async Task<ActionResult<FixedExpenseDto>> GetFixedExpenseById(Guid id)
    {
        var item = await _expenseService.GetFixedExpenseByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost("fixed")]
    public async Task<ActionResult<FixedExpenseDto>> CreateFixedExpense([FromBody] CreateFixedExpenseDto dto)
    {
        var created = await _expenseService.CreateFixedExpenseAsync(dto);
        return CreatedAtAction(nameof(GetFixedExpenseById), new { id = created.Id }, created);
    }

    [HttpPut("fixed/{id:guid}")]
    public async Task<ActionResult<FixedExpenseDto>> UpdateFixedExpense(Guid id, [FromBody] CreateFixedExpenseDto dto)
    {
        var updated = await _expenseService.UpdateFixedExpenseAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("fixed/{id:guid}")]
    public async Task<IActionResult> DeleteFixedExpense(Guid id)
    {
        var deleted = await _expenseService.DeleteFixedExpenseAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ==========================================
    // Reports
    // ==========================================

    [HttpGet("reports/monthly")]
    public async Task<ActionResult<MonthlyExpenseReportDto>> GetMonthlyReport(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber)
    {
        var (y, m) = ParseMonthQuery(month, year, monthNumber);
        var yVal = y ?? DateTime.Now.Year;
        var mVal = m ?? DateTime.Now.Month;
        var report = await _expenseService.GetMonthlyExpenseReportAsync(yVal, mVal);
        return Ok(report);
    }

    [HttpGet("reports/segregated")]
    public async Task<ActionResult<SegregatedSpendingReportDto>> GetSegregatedReport(
        [FromQuery] string? month,
        [FromQuery] int? year,
        [FromQuery] int? monthNumber,
        [FromQuery] Guid? categoryId)
    {
        var (y, m) = ParseMonthQuery(month, year, monthNumber);
        var yVal = y ?? DateTime.Now.Year;
        var mVal = m ?? DateTime.Now.Month;
        var report = await _expenseService.GetSegregatedReportAsync(yVal, mVal, categoryId);
        return Ok(report);
    }
}
