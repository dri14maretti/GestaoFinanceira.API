using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.StoredMoney;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundsController : ControllerBase
{
    private readonly StoredMoneyService _storedMoneyService;

    public FundsController(StoredMoneyService storedMoneyService)
    {
        _storedMoneyService = storedMoneyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoredMoneyFundDto>>> GetAll()
    {
        var list = await _storedMoneyService.GetAllFundsAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoredMoneyFundDto>> GetById(Guid id)
    {
        var item = await _storedMoneyService.GetFundByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<StoredMoneyFundDto>> Create([FromBody] CreateStoredMoneyFundDto dto)
    {
        var created = await _storedMoneyService.CreateFundAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StoredMoneyFundDto>> Update(Guid id, [FromBody] CreateStoredMoneyFundDto dto)
    {
        var updated = await _storedMoneyService.UpdateFundAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _storedMoneyService.DeleteFundAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // --- Transactions ---

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<IEnumerable<FundTransactionDto>>> GetTransactions(Guid id)
    {
        var list = await _storedMoneyService.GetTransactionsByFundAsync(id);
        return Ok(list);
    }

    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<FundTransactionDto>> CreateTransaction(Guid id, [FromBody] CreateFundTransactionDto dto)
    {
        var payload = dto with { FundId = id };
        var created = await _storedMoneyService.CreateTransactionAsync(payload);
        return Ok(created);
    }
}
