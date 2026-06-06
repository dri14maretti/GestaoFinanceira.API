using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Planning;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Services;

public class PlanningService
{
    private readonly IMonthlyPlanRepository _planRepo;
    private readonly IPlanAllocationRepository _allocationRepo;
    private readonly IPlanningCategoryRepository _categoryRepo;

    public PlanningService(
        IMonthlyPlanRepository planRepo,
        IPlanAllocationRepository allocationRepo,
        IPlanningCategoryRepository categoryRepo)
    {
        _planRepo = planRepo;
        _allocationRepo = allocationRepo;
        _categoryRepo = categoryRepo;
    }

    // --- Plans ---

    public async Task<MonthlyPlanDto?> GetPlanByMonthAsync(int year, int month)
    {
        var plan = await _planRepo.GetByMonthAsync(year, month);
        return plan == null ? null : await MapToPlanDtoAsync(plan);
    }

    public async Task<MonthlyPlanDto> GetOrCreatePlanAsync(int year, int month, decimal income)
    {
        var plan = await _planRepo.GetByMonthAsync(year, month);
        if (plan == null)
        {
            plan = new MonthlyPlan(new DateTime(year, month, 1), income);
            await _planRepo.AddAsync(plan);
        }
        return await MapToPlanDtoAsync(plan);
    }

    public async Task<MonthlyPlanDto?> UpdatePlanIncomeAsync(Guid id, decimal income)
    {
        var plan = await _planRepo.GetByIdAsync(id);
        if (plan == null) return null;
        plan.UpdateIncome(income);
        await _planRepo.UpdateAsync(plan);
        return await MapToPlanDtoAsync(plan);
    }

    public async Task<IEnumerable<MonthlyPlanDto>> GetAllPlansAsync()
    {
        var plans = await _planRepo.GetAllAsync();
        var list = new List<MonthlyPlanDto>();
        foreach (var plan in plans)
        {
            list.Add(await MapToPlanDtoAsync(plan));
        }
        return list;
    }

    public async Task<bool> DeletePlanAsync(Guid id) => await _planRepo.DeleteAsync(id);

    // --- Allocations ---

    public async Task<PlanAllocationDto> CreateAllocationAsync(CreatePlanAllocationDto dto)
    {
        var allocation = new PlanAllocation(dto.MonthlyPlanId, dto.PlanningCategoryId, dto.AllocatedValue);
        await _allocationRepo.AddAsync(allocation);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToAllocationDto(allocation, categories);
    }

    public async Task<PlanAllocationDto?> UpdateAllocationAsync(Guid id, decimal allocatedValue)
    {
        var allocation = await _allocationRepo.GetByIdAsync(id);
        if (allocation == null) return null;
        allocation.UpdateAllocatedValue(allocatedValue);
        await _allocationRepo.UpdateAsync(allocation);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToAllocationDto(allocation, categories);
    }

    public async Task<bool> DeleteAllocationAsync(Guid id) => await _allocationRepo.DeleteAsync(id);

    public async Task<IEnumerable<PlanAllocationDto>> GetAllocationsByPlanAsync(Guid planId)
    {
        var allocations = await _allocationRepo.GetByPlanIdAsync(planId);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return allocations.Select(a => MapToAllocationDto(a, categories)).ToList();
    }

    // --- Categories ---

    public async Task<IEnumerable<PlanningCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();
        return categories.Select(c => new PlanningCategoryDto(c.Id, c.Name));
    }

    public async Task<PlanningCategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var entity = await _categoryRepo.GetByIdAsync(id);
        return entity == null ? null : new PlanningCategoryDto(entity.Id, entity.Name);
    }

    public async Task<PlanningCategoryDto> CreateCategoryAsync(CreatePlanningCategoryDto dto)
    {
        var entity = new PlanningCategory(dto.Name);
        await _categoryRepo.AddAsync(entity);
        return new PlanningCategoryDto(entity.Id, entity.Name);
    }

    public async Task<PlanningCategoryDto?> UpdateCategoryAsync(Guid id, CreatePlanningCategoryDto dto)
    {
        var entity = await _categoryRepo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Update(dto.Name);
        await _categoryRepo.UpdateAsync(entity);
        return new PlanningCategoryDto(entity.Id, entity.Name);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id) => await _categoryRepo.DeleteAsync(id);

    // --- Mapping ---

    private async Task<MonthlyPlanDto> MapToPlanDtoAsync(MonthlyPlan plan)
    {
        var allocations = await _allocationRepo.GetByPlanIdAsync(plan.Id);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var mappedAllocations = allocations.Select(a => MapToAllocationDto(a, categories)).ToList();
        var totalAllocated = mappedAllocations.Sum(a => a.AllocatedValue);
        return new MonthlyPlanDto(plan.Id, plan.Month, plan.Income, mappedAllocations, totalAllocated, plan.Income - totalAllocated);
    }

    private static PlanAllocationDto MapToAllocationDto(PlanAllocation allocation, Dictionary<Guid, PlanningCategory> categories)
    {
        var categoryName = categories.TryGetValue(allocation.PlanningCategoryId, out var cat) ? cat.Name : "Desconhecida";
        return new PlanAllocationDto(allocation.Id, allocation.MonthlyPlanId, allocation.PlanningCategoryId, categoryName, allocation.AllocatedValue);
    }
}
