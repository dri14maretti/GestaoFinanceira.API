using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Expenses;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;

namespace GerenciamentoFinanceiro.Application.Services;

public class ExpenseService
{
    private readonly ICreditCardSpendingRepository _ccSpendingRepo;
    private readonly ICashExpenseRepository _cashExpenseRepo;
    private readonly IInstallmentRepository _installmentRepo;
    private readonly IFixedExpenseRepository _fixedExpenseRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ICreditCardRepository _creditCardRepo;

    public ExpenseService(
        ICreditCardSpendingRepository ccSpendingRepo,
        ICashExpenseRepository cashExpenseRepo,
        IInstallmentRepository installmentRepo,
        IFixedExpenseRepository fixedExpenseRepo,
        ICategoryRepository categoryRepo,
        ICreditCardRepository creditCardRepo)
    {
        _ccSpendingRepo = ccSpendingRepo;
        _cashExpenseRepo = cashExpenseRepo;
        _installmentRepo = installmentRepo;
        _fixedExpenseRepo = fixedExpenseRepo;
        _categoryRepo = categoryRepo;
        _creditCardRepo = creditCardRepo;
    }

    // --- Credit Card Spendings ---

    public async Task<IEnumerable<CreditCardSpendingDto>> GetAllCreditCardSpendingsAsync(int? year, int? month, Guid? creditCardId, Guid? categoryId)
    {
        IEnumerable<CreditCardSpending> spendings;
        if (year.HasValue && month.HasValue)
            spendings = await _ccSpendingRepo.GetByMonthAsync(year.Value, month.Value);
        else
            spendings = await _ccSpendingRepo.GetAllAsync();

        if (creditCardId.HasValue)
            spendings = spendings.Where(s => s.CreditCardId == creditCardId.Value);
        if (categoryId.HasValue)
            spendings = spendings.Where(s => s.CategoryId == categoryId.Value);

        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return spendings.Select(s => MapToCreditCardSpendingDto(s, categories, creditCards)).ToList();
    }

    public async Task<CreditCardSpendingDto?> GetCreditCardSpendingByIdAsync(Guid id)
    {
        var entity = await _ccSpendingRepo.GetByIdAsync(id);
        if (entity == null) return null;
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCreditCardSpendingDto(entity, categories, creditCards);
    }

    public async Task<CreditCardSpendingDto> CreateCreditCardSpendingAsync(CreateCreditCardSpendingDto dto)
    {
        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' was not found.");
        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        var entity = new CreditCardSpending(dto.Value, dto.CategoryId, dto.CreditCardId, dto.Date, dto.Description);
        await _ccSpendingRepo.AddAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCreditCardSpendingDto(entity, categories, creditCards);
    }

    public async Task<CreditCardSpendingDto?> UpdateCreditCardSpendingAsync(Guid id, CreateCreditCardSpendingDto dto)
    {
        var entity = await _ccSpendingRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' was not found.");
        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        entity.Update(dto.Value, dto.CategoryId, dto.CreditCardId, dto.Date, dto.Description);
        await _ccSpendingRepo.UpdateAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCreditCardSpendingDto(entity, categories, creditCards);
    }

    public async Task<bool> DeleteCreditCardSpendingAsync(Guid id) => await _ccSpendingRepo.DeleteAsync(id);

    // --- Cash Expenses ---

    public async Task<IEnumerable<CashExpenseDto>> GetAllCashExpensesAsync(int? year, int? month, string? bank, Guid? categoryId)
    {
        IEnumerable<CashExpense> expenses;
        if (year.HasValue && month.HasValue)
            expenses = await _cashExpenseRepo.GetByMonthAsync(year.Value, month.Value);
        else
            expenses = await _cashExpenseRepo.GetAllAsync();

        if (!string.IsNullOrEmpty(bank))
            expenses = expenses.Where(e => e.Bank.Equals(bank, StringComparison.OrdinalIgnoreCase));
        if (categoryId.HasValue)
            expenses = expenses.Where(e => e.CategoryId == categoryId.Value);

        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return expenses.Select(e => MapToCashExpenseDto(e, categories)).ToList();
    }

    public async Task<CashExpenseDto?> GetCashExpenseByIdAsync(Guid id)
    {
        var entity = await _cashExpenseRepo.GetByIdAsync(id);
        if (entity == null) return null;
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCashExpenseDto(entity, categories);
    }

    public async Task<CashExpenseDto> CreateCashExpenseAsync(CreateCashExpenseDto dto)
    {
        if (dto.CategoryId.HasValue)
        {
            _ = await _categoryRepo.GetByIdAsync(dto.CategoryId.Value)
                ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId.Value}' was not found.");
        }

        var entity = new CashExpense(dto.Value, dto.CategoryId, dto.Bank, dto.Date, dto.Description);
        await _cashExpenseRepo.AddAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCashExpenseDto(entity, categories);
    }

    public async Task<CashExpenseDto?> UpdateCashExpenseAsync(Guid id, CreateCashExpenseDto dto)
    {
        var entity = await _cashExpenseRepo.GetByIdAsync(id);
        if (entity == null) return null;

        if (dto.CategoryId.HasValue)
        {
            _ = await _categoryRepo.GetByIdAsync(dto.CategoryId.Value)
                ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId.Value}' was not found.");
        }

        entity.Update(dto.Value, dto.CategoryId, dto.Bank, dto.Date, dto.Description);
        await _cashExpenseRepo.UpdateAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToCashExpenseDto(entity, categories);
    }

    public async Task<bool> DeleteCashExpenseAsync(Guid id) => await _cashExpenseRepo.DeleteAsync(id);

    public async Task<IEnumerable<BankBalanceDto>> GetBankBalancesAsync()
    {
        var balances = await _cashExpenseRepo.GetBalancesByBankAsync();
        return balances
            .OrderBy(b => b.Key)
            .Select(b => new BankBalanceDto(b.Key, b.Value))
            .ToList();
    }

    // --- Installments ---

    public async Task<IEnumerable<InstallmentDto>> GetAllInstallmentsAsync(int? year, int? month, Guid? creditCardId)
    {
        IEnumerable<Installment> installments;
        if (year.HasValue && month.HasValue)
            installments = await _installmentRepo.GetByMonthAsync(year.Value, month.Value);
        else
            installments = await _installmentRepo.GetAllAsync();

        if (creditCardId.HasValue)
            installments = installments.Where(i => i.CreditCardId == creditCardId.Value);

        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return installments.Select(i => MapToInstallmentDto(i, categories, creditCards)).ToList();
    }

    public async Task<InstallmentDto?> GetInstallmentByIdAsync(Guid id)
    {
        var entity = await _installmentRepo.GetByIdAsync(id);
        if (entity == null) return null;
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInstallmentDto(entity, categories, creditCards);
    }

    public async Task<InstallmentDto> CreateInstallmentAsync(CreateInstallmentDto dto)
    {
        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' was not found.");
        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        var entity = new Installment(dto.Value, dto.CategoryId, dto.CreditCardId, dto.Month, dto.Description, dto.TotalInstallments, dto.CurrentInstallment);
        await _installmentRepo.AddAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInstallmentDto(entity, categories, creditCards);
    }

    public async Task<InstallmentDto?> UpdateInstallmentAsync(Guid id, CreateInstallmentDto dto)
    {
        var entity = await _installmentRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException($"Category with ID '{dto.CategoryId}' was not found.");
        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        entity.Update(dto.Value, dto.CategoryId, dto.CreditCardId, dto.Month, dto.Description, dto.TotalInstallments, dto.CurrentInstallment);
        await _installmentRepo.UpdateAsync(entity);
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToInstallmentDto(entity, categories, creditCards);
    }

    public async Task<bool> DeleteInstallmentAsync(Guid id) => await _installmentRepo.DeleteAsync(id);

    // --- Fixed Expenses ---

    public async Task<IEnumerable<FixedExpenseDto>> GetAllFixedExpensesAsync(Guid? creditCardId)
    {
        IEnumerable<FixedExpense> expenses;
        if (creditCardId.HasValue)
            expenses = await _fixedExpenseRepo.GetByCreditCardIdAsync(creditCardId.Value);
        else
            expenses = await _fixedExpenseRepo.GetAllAsync();

        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);

        return expenses.Select(e => MapToFixedExpenseDto(e, creditCards)).ToList();
    }

    public async Task<FixedExpenseDto?> GetFixedExpenseByIdAsync(Guid id)
    {
        var entity = await _fixedExpenseRepo.GetByIdAsync(id);
        if (entity == null) return null;
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToFixedExpenseDto(entity, creditCards);
    }

    public async Task<FixedExpenseDto> CreateFixedExpenseAsync(CreateFixedExpenseDto dto)
    {
        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        var entity = new FixedExpense(dto.Value, dto.Description, dto.CreditCardId);
        await _fixedExpenseRepo.AddAsync(entity);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToFixedExpenseDto(entity, creditCards);
    }

    public async Task<FixedExpenseDto?> UpdateFixedExpenseAsync(Guid id, CreateFixedExpenseDto dto)
    {
        var entity = await _fixedExpenseRepo.GetByIdAsync(id);
        if (entity == null) return null;

        var card = await _creditCardRepo.GetByIdAsync(dto.CreditCardId)
            ?? throw new KeyNotFoundException($"Credit card with ID '{dto.CreditCardId}' was not found.");

        entity.Update(dto.Value, dto.Description, dto.CreditCardId);
        await _fixedExpenseRepo.UpdateAsync(entity);
        var creditCards = (await _creditCardRepo.GetAllAsync()).ToDictionary(c => c.Id);
        return MapToFixedExpenseDto(entity, creditCards);
    }

    public async Task<bool> DeleteFixedExpenseAsync(Guid id) => await _fixedExpenseRepo.DeleteAsync(id);

    // --- Reports ---

    public async Task<MonthlyExpenseReportDto> GetMonthlyExpenseReportAsync(int year, int month)
    {
        var ccSpendings = await _ccSpendingRepo.GetByMonthAsync(year, month);
        var cashExpenses = await _cashExpenseRepo.GetByMonthAsync(year, month);
        var installments = await _installmentRepo.GetByMonthAsync(year, month);
        var fixedExpenses = await _fixedExpenseRepo.GetAllAsync();
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        var allCategoryIds = ccSpendings.Select(s => s.CategoryId)
            .Concat(cashExpenses.Where(e => e.CategoryId.HasValue).Select(e => e.CategoryId!.Value))
            .Concat(installments.Select(i => i.CategoryId))
            .Distinct();

        var totalFixed = fixedExpenses.Sum(f => f.Value);

        var breakdowns = allCategoryIds.Select(catId =>
        {
            var catName = categories.GetValueOrDefault(catId, "Desconhecida");
            var ccTotal = ccSpendings.Where(s => s.CategoryId == catId).Sum(s => s.Value);
            var cashTotal = cashExpenses.Where(e => e.CategoryId == catId).Sum(e => e.Value < 0 ? Math.Abs(e.Value) : e.Value);
            var instTotal = installments.Where(i => i.CategoryId == catId).Sum(i => i.Value);
            var fixedTotal = 0m;
            return new CategoryExpenseBreakdownDto(catName, ccTotal, cashTotal, instTotal, fixedTotal, ccTotal + cashTotal + instTotal + fixedTotal);
        }).ToList();

        if (totalFixed > 0)
        {
            breakdowns.Add(new CategoryExpenseBreakdownDto(
                "Despesas Fixas",
                CreditCardTotal: 0m,
                CashTotal: 0m,
                InstallmentTotal: 0m,
                FixedTotal: totalFixed,
                Total: totalFixed
            ));
        }

        breakdowns = breakdowns.OrderByDescending(b => b.Total).ToList();

        var totalExpenses = breakdowns.Sum(b => b.Total);

        return new MonthlyExpenseReportDto(year, month, totalExpenses, breakdowns);
    }

    public async Task<SegregatedSpendingReportDto> GetSegregatedReportAsync(int year, int month, Guid? categoryId)
    {
        var report = await GetMonthlyExpenseReportAsync(year, month);
        
        string? categoryName = null;
        if (categoryId.HasValue)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId.Value);
            categoryName = category?.Name;
        }

        var categories = categoryName != null
            ? report.Categories.Where(c => c.CategoryName == categoryName).ToList()
            : report.Categories;

        return new SegregatedSpendingReportDto(
            year, month,
            categories.Sum(c => c.CreditCardTotal),
            categories.Sum(c => c.CashTotal),
            categories.Sum(c => c.InstallmentTotal),
            categories.Sum(c => c.FixedTotal),
            categories.Sum(c => c.Total),
            categories
        );
    }

    // --- Mapping Helpers ---

    private CreditCardSpendingDto MapToCreditCardSpendingDto(
        CreditCardSpending entity,
        Dictionary<Guid, Category> categories,
        Dictionary<Guid, CreditCard> creditCards)
    {
        var categoryName = categories.TryGetValue(entity.CategoryId, out var cat) ? cat.Name : "Desconhecida";
        var cardBankName = creditCards.TryGetValue(entity.CreditCardId, out var cc) ? cc.BankName : "Desconhecido";
        return new CreditCardSpendingDto(entity.Id, entity.Value, entity.CategoryId, categoryName, entity.CreditCardId, cardBankName, entity.Date, entity.Description);
    }

    private CashExpenseDto MapToCashExpenseDto(
        CashExpense entity,
        Dictionary<Guid, Category> categories)
    {
        string? categoryName = null;
        if (entity.CategoryId.HasValue)
        {
            categoryName = categories.TryGetValue(entity.CategoryId.Value, out var cat) ? cat.Name : "Desconhecida";
        }
        return new CashExpenseDto(entity.Id, entity.Value, entity.CategoryId, categoryName, entity.Bank, entity.Date, entity.Description);
    }

    private InstallmentDto MapToInstallmentDto(
        Installment entity,
        Dictionary<Guid, Category> categories,
        Dictionary<Guid, CreditCard> creditCards)
    {
        var categoryName = categories.TryGetValue(entity.CategoryId, out var cat) ? cat.Name : "Desconhecida";
        var cardBankName = creditCards.TryGetValue(entity.CreditCardId, out var cc) ? cc.BankName : "Desconhecido";
        return new InstallmentDto(entity.Id, entity.Value, entity.CategoryId, categoryName, entity.CreditCardId, cardBankName, entity.Month, entity.Description, entity.TotalInstallments, entity.CurrentInstallment);
    }

    private FixedExpenseDto MapToFixedExpenseDto(
        FixedExpense entity,
        Dictionary<Guid, CreditCard> creditCards)
    {
        var cardBankName = creditCards.TryGetValue(entity.CreditCardId, out var cc) ? cc.BankName : "Desconhecido";
        return new FixedExpenseDto(entity.Id, entity.Value, entity.Description, entity.CreditCardId, cardBankName);
    }
}
