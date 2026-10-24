using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Investments;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.Services;

public class InvestmentService
{
    private readonly IInvestmentRepository _investmentRepo;
    private readonly IInvestmentTransactionRepository _transactionRepo;
    private readonly IInvestmentCategoryRepository _categoryRepo;

    public InvestmentService(
        IInvestmentRepository investmentRepo,
        IInvestmentTransactionRepository transactionRepo,
        IInvestmentCategoryRepository categoryRepo)
    {
        _investmentRepo = investmentRepo;
        _transactionRepo = transactionRepo;
        _categoryRepo = categoryRepo;
    }

    // --- Investments ---

    public async Task<IEnumerable<InvestmentDto>> GetAllInvestmentsAsync()
    {
        var investments = await _investmentRepo.GetAllAsync();
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return investments.Select(i => MapToInvestmentDto(i, categories)).ToList();
    }

    public async Task<InvestmentDto?> GetInvestmentByIdAsync(Guid id)
    {
        var entity = await _investmentRepo.GetByIdAsync(id);
        if (entity == null) return null;
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInvestmentDto(entity, categories);
    }

    public async Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto dto)
    {
        var cat = await _categoryRepo.GetByIdAsync(dto.InvestmentCategoryId)
            ?? throw new KeyNotFoundException($"Investment category with ID '{dto.InvestmentCategoryId}' was not found.");

        var entity = new Investment(dto.Value, dto.Date, dto.InvestmentCategoryId);
        await _investmentRepo.AddAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInvestmentDto(entity, categories);
    }

    public async Task<InvestmentDto?> UpdateInvestmentAsync(Guid id, CreateInvestmentDto dto)
    {
        var entity = await _investmentRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var cat = await _categoryRepo.GetByIdAsync(dto.InvestmentCategoryId)
            ?? throw new KeyNotFoundException($"Investment category with ID '{dto.InvestmentCategoryId}' was not found.");

        entity.Update(dto.Value, dto.Date, dto.InvestmentCategoryId);
        await _investmentRepo.UpdateAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInvestmentDto(entity, categories);
    }

    public async Task<bool> DeleteInvestmentAsync(Guid id) => await _investmentRepo.DeleteAsync(id);

    // --- Transactions ---

    public async Task<IEnumerable<InvestmentTransactionDto>> GetTransactionsByInvestmentAsync(Guid investmentId)
    {
        var transactions = await _transactionRepo.GetByInvestmentIdAsync(investmentId);
        return transactions.Select(MapToTransactionDto).ToList();
    }

    public async Task<InvestmentTransactionDto> CreateTransactionAsync(CreateInvestmentTransactionDto dto)
    {
        var investment = await _investmentRepo.GetByIdAsync(dto.InvestmentId)
            ?? throw new KeyNotFoundException($"Investment with ID '{dto.InvestmentId}' was not found.");

        var entity = new InvestmentTransaction(dto.Type, dto.Value, dto.Date, dto.InvestmentId);
        await _transactionRepo.AddAsync(entity);

        // Update investment value based on transaction type
        switch (dto.Type)
        {
            case InvestmentTransactionType.Deposit:
                investment.Update(investment.Value + dto.Value, investment.Date, investment.InvestmentCategoryId);
                break;
            case InvestmentTransactionType.Withdrawal:
                investment.Update(investment.Value - dto.Value, investment.Date, investment.InvestmentCategoryId);
                break;
            case InvestmentTransactionType.Rentability:
                investment.ApplyRentability(dto.Value);
                break;
        }
        await _investmentRepo.UpdateAsync(investment);

        return MapToTransactionDto(entity);
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        var tx = await _transactionRepo.GetByIdAsync(id);
        if (tx == null) return false;

        var investment = await _investmentRepo.GetByIdAsync(tx.InvestmentId);
        if (investment != null)
        {
            switch (tx.Type)
            {
                case InvestmentTransactionType.Deposit:
                    if (investment.Value - tx.Value < 0)
                        throw new InvalidOperationException("Cannot delete deposit transaction: resulting investment balance would be negative.");
                    investment.Update(investment.Value - tx.Value, investment.Date, investment.InvestmentCategoryId);
                    break;

                case InvestmentTransactionType.Withdrawal:
                    investment.Update(investment.Value + tx.Value, investment.Date, investment.InvestmentCategoryId);
                    break;

                case InvestmentTransactionType.Rentability:
                    if (investment.Value - tx.Value < 0)
                        throw new InvalidOperationException("Cannot delete rentability transaction: resulting investment balance would be negative.");
                    investment.ApplyRentability(-tx.Value);
                    break;
            }

            await _investmentRepo.UpdateAsync(investment);
        }

        return await _transactionRepo.DeleteAsync(id);
    }

    // --- Categories ---

    public async Task<IEnumerable<InvestmentCategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();
        return categories.Select(c => new InvestmentCategoryDto(c.Id, c.Name)).ToList();
    }

    public async Task<InvestmentCategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var entity = await _categoryRepo.GetByIdAsync(id);
        return entity == null ? null : new InvestmentCategoryDto(entity.Id, entity.Name);
    }

    public async Task<InvestmentCategoryDto> CreateCategoryAsync(CreateInvestmentCategoryDto dto)
    {
        var entity = new InvestmentCategory(dto.Name);
        await _categoryRepo.AddAsync(entity);
        return new InvestmentCategoryDto(entity.Id, entity.Name);
    }

    public async Task<InvestmentCategoryDto?> UpdateCategoryAsync(Guid id, CreateInvestmentCategoryDto dto)
    {
        var entity = await _categoryRepo.GetByIdAsync(id);
        if (entity == null) return null;
        entity.Update(dto.Name);
        await _categoryRepo.UpdateAsync(entity);
        return new InvestmentCategoryDto(entity.Id, entity.Name);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id) => await _categoryRepo.DeleteAsync(id);

    // --- Reports ---

    public async Task<AnnualInvestmentReportDto> GetAnnualReportAsync(int year)
    {
        var transactions = await _transactionRepo.GetByYearAsync(year);
        var totalInvestments = await _investmentRepo.GetTotalValueAsync();

        var monthlyBreakdown = Enumerable.Range(1, 12).Select(month =>
        {
            var monthTransactions = transactions.Where(t => t.Date.Month == month);
            var deposits = monthTransactions.Where(t => t.Type == InvestmentTransactionType.Deposit).Sum(t => t.Value);
            var withdrawals = monthTransactions.Where(t => t.Type == InvestmentTransactionType.Withdrawal).Sum(t => t.Value);
            var rentability = monthTransactions.Where(t => t.Type == InvestmentTransactionType.Rentability).Sum(t => t.Value);
            return new MonthlyInvestmentBreakdownDto(month, deposits, withdrawals, rentability, deposits - withdrawals + rentability);
        }).ToList();

        return new AnnualInvestmentReportDto(
            year,
            monthlyBreakdown.Sum(m => m.Deposits),
            monthlyBreakdown.Sum(m => m.Withdrawals),
            monthlyBreakdown.Sum(m => m.Rentability),
            totalInvestments,
            monthlyBreakdown
        );
    }

    // --- Mapping ---

    private static InvestmentDto MapToInvestmentDto(Investment entity, Dictionary<Guid, InvestmentCategory> categories)
    {
        var categoryName = categories.TryGetValue(entity.InvestmentCategoryId, out var cat) ? cat.Name : "Desconhecida";
        return new InvestmentDto(entity.Id, entity.Value, entity.Date, entity.InvestmentCategoryId, categoryName);
    }

    private static InvestmentTransactionDto MapToTransactionDto(InvestmentTransaction entity) =>
        new(entity.Id, entity.Type, entity.Value, entity.Date, entity.InvestmentId);
}
