using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/investment-categories")]
public class InvestmentCategoriesController : ControllerBase
{
    private readonly InvestmentService _investmentService;

    public InvestmentCategoriesController(InvestmentService investmentService)
    {
        _investmentService = investmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvestmentCategoryDto>>> GetAll()
    {
        var categories = await _investmentService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvestmentCategoryDto>> GetById(Guid id)
    {
        var category = await _investmentService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<InvestmentCategoryDto>> Create([FromBody] CreateInvestmentCategoryDto dto)
    {
        var created = await _investmentService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvestmentCategoryDto>> Update(Guid id, [FromBody] CreateInvestmentCategoryDto dto)
    {
        var updated = await _investmentService.UpdateCategoryAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _investmentService.DeleteCategoryAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
