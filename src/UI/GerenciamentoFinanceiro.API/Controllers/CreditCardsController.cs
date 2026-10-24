using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Categories;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/credit-cards")]
public class CreditCardsController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CreditCardsController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetAll()
    {
        var cards = await _categoryService.GetAllCreditCardsAsync();
        return Ok(cards);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CreditCardDto>> GetById(Guid id)
    {
        var card = await _categoryService.GetCreditCardByIdAsync(id);
        if (card == null) return NotFound();
        return Ok(card);
    }

    [HttpPost]
    public async Task<ActionResult<CreditCardDto>> Create([FromBody] CreateCreditCardDto dto)
    {
        var created = await _categoryService.CreateCreditCardAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CreditCardDto>> Update(Guid id, [FromBody] CreateCreditCardDto dto)
    {
        var updated = await _categoryService.UpdateCreditCardAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _categoryService.DeleteCreditCardAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
