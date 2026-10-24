using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Categories;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly ICreditCardRepository _creditCardRepo;

    public CategoryService(ICategoryRepository categoryRepo, ICreditCardRepository creditCardRepo)
    {
        _categoryRepo = categoryRepo;
        _creditCardRepo = creditCardRepo;
    }

    // --- Categories ---

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CategoryType? type = null)
    {
        var categories = type.HasValue
            ? await _categoryRepo.GetByTypeAsync(type.Value)
            : await _categoryRepo.GetAllAsync();

        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Type)).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        return category == null ? null : new CategoryDto(category.Id, category.Name, category.Type);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var entity = new Category(dto.Name, dto.Type);
        await _categoryRepo.AddAsync(entity);
        return new CategoryDto(entity.Id, entity.Name, entity.Type);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, CreateCategoryDto dto)
    {
        var entity = await _categoryRepo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Update(dto.Name, dto.Type);
        await _categoryRepo.UpdateAsync(entity);
        return new CategoryDto(entity.Id, entity.Name, entity.Type);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id) => await _categoryRepo.DeleteAsync(id);

    // --- Credit Cards ---

    public async Task<IEnumerable<CreditCardDto>> GetAllCreditCardsAsync()
    {
        var cards = await _creditCardRepo.GetAllAsync();
        return cards.Select(c => new CreditCardDto(c.Id, c.BankName, c.ClosingDate, c.DueDate)).ToList();
    }

    public async Task<CreditCardDto?> GetCreditCardByIdAsync(Guid id)
    {
        var card = await _creditCardRepo.GetByIdAsync(id);
        return card == null ? null : new CreditCardDto(card.Id, card.BankName, card.ClosingDate, card.DueDate);
    }

    public async Task<CreditCardDto> CreateCreditCardAsync(CreateCreditCardDto dto)
    {
        var entity = new CreditCard(dto.BankName, dto.ClosingDate, dto.DueDate);
        await _creditCardRepo.AddAsync(entity);
        return new CreditCardDto(entity.Id, entity.BankName, entity.ClosingDate, entity.DueDate);
    }

    public async Task<CreditCardDto?> UpdateCreditCardAsync(Guid id, CreateCreditCardDto dto)
    {
        var entity = await _creditCardRepo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Update(dto.BankName, dto.ClosingDate, dto.DueDate);
        await _creditCardRepo.UpdateAsync(entity);
        return new CreditCardDto(entity.Id, entity.BankName, entity.ClosingDate, entity.DueDate);
    }

    public async Task<bool> DeleteCreditCardAsync(Guid id) => await _creditCardRepo.DeleteAsync(id);
}
