using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/planning-categories")]
public class PlanningCategoriesController : ControllerBase
{
    private readonly PlanningService _planningService;

    public PlanningCategoriesController(PlanningService planningService)
    {
        _planningService = planningService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanningCategoryDto>>> GetAll()
    {
        var categories = await _planningService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlanningCategoryDto>> GetById(Guid id)
    {
        var category = await _planningService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<PlanningCategoryDto>> Create([FromBody] CreatePlanningCategoryDto dto)
    {
        var created = await _planningService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlanningCategoryDto>> Update(Guid id, [FromBody] CreatePlanningCategoryDto dto)
    {
        var updated = await _planningService.UpdateCategoryAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _planningService.DeleteCategoryAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
