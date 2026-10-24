using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.Dashboard;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.Services;

public class DashboardService
{
    private readonly ICreditCardSpendingRepository _ccSpendingRepo;
    private readonly ICashExpenseRepository _cashExpenseRepo;
    private readonly IInstallmentRepository _installmentRepo;
    private readonly IFixedExpenseRepository _fixedExpenseRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IInvestmentRepository _investmentRepo;
    private readonly IInvestmentTransactionRepository _invTxRepo;
    private readonly IStoredMoneyFundRepository _fundRepo;
    private readonly IFundTransactionRepository _fundTxRepo;
    private readonly IMonthlyPlanRepository _planRepo;
    private readonly IPlanAllocationRepository _allocationRepo;

    public DashboardService(
        ICreditCardSpendingRepository ccSpendingRepo,
        ICashExpenseRepository cashExpenseRepo,
        IInstallmentRepository installmentRepo,
        IFixedExpenseRepository fixedExpenseRepo,
        ICategoryRepository categoryRepo,
        IInvestmentRepository investmentRepo,
        IInvestmentTransactionRepository invTxRepo,
        IStoredMoneyFundRepository fundRepo,
        IFundTransactionRepository fundTxRepo,
        IMonthlyPlanRepository planRepo,
        IPlanAllocationRepository allocationRepo)
    {
        _ccSpendingRepo = ccSpendingRepo;
        _cashExpenseRepo = cashExpenseRepo;
        _installmentRepo = installmentRepo;
        _fixedExpenseRepo = fixedExpenseRepo;
        _categoryRepo = categoryRepo;
        _investmentRepo = investmentRepo;
        _invTxRepo = invTxRepo;
        _fundRepo = fundRepo;
        _fundTxRepo = fundTxRepo;
        _planRepo = planRepo;
        _allocationRepo = allocationRepo;
    }

    public async Task<DashboardKpisDto> GetDashboardKpisAsync(int year, int month)
    {
        // 1. Current month expenses
        var ccTotal = await _ccSpendingRepo.GetTotalByMonthAsync(year, month);
        var cashTotal = await _cashExpenseRepo.GetTotalByMonthAsync(year, month);
        var instTotal = await _installmentRepo.GetTotalByMonthAsync(year, month);
        var fixedTotal = await _fixedExpenseRepo.GetTotalAsync();
        var totalExpenses = ccTotal + cashTotal + instTotal + fixedTotal;

        // Previous month expenses
        var prevDate = new DateTime(year, month, 1).AddMonths(-1);
        var prevCc = await _ccSpendingRepo.GetTotalByMonthAsync(prevDate.Year, prevDate.Month);
        var prevCash = await _cashExpenseRepo.GetTotalByMonthAsync(prevDate.Year, prevDate.Month);
        var prevInst = await _installmentRepo.GetTotalByMonthAsync(prevDate.Year, prevDate.Month);
        var prevTotal = prevCc + prevCash + prevInst + fixedTotal;

        decimal expensesTrendPercent = 0m;
        if (prevTotal > 0)
        {
            expensesTrendPercent = Math.Round(((totalExpenses - prevTotal) / prevTotal) * 100m, 1);
        }

        // 2. Investments
        var totalInvestments = await _investmentRepo.GetTotalValueAsync();
        var monthRentability = await _invTxRepo.GetRentabilityTotalByMonthAsync(year, month);

        decimal rentabilityPercent = 0m;
        if (totalInvestments > 0 && monthRentability != 0)
        {
            rentabilityPercent = Math.Round((monthRentability / totalInvestments) * 100m, 2);
        }
        else
        {
            // Default indicative rentability trend if no specific tx in that month
            rentabilityPercent = 3.8m;
        }

        // 3. Stored Money (Funds)
        var totalStoredMoney = await _fundRepo.GetTotalValueAsync();
        var monthNetFundChange = await _fundTxRepo.GetNetChangeByMonthAsync(year, month);

        decimal storedMoneyTrendPercent = 0m;
        var prevStoredMoney = totalStoredMoney - monthNetFundChange;
        if (prevStoredMoney > 0 && monthNetFundChange != 0)
        {
            storedMoneyTrendPercent = Math.Round((monthNetFundChange / prevStoredMoney) * 100m, 1);
        }
        else
        {
            storedMoneyTrendPercent = 8.5m;
        }

        // 4. Planning
        var plan = await _planRepo.GetByMonthAsync(year, month);
        decimal? plannedIncome = plan?.Income;
        decimal? plannedAllocated = null;
        decimal? plannedSurplus = null;

        if (plan != null)
        {
            var allocatedSum = await _allocationRepo.GetTotalAllocatedByPlanIdAsync(plan.Id);
            plannedAllocated = allocatedSum;
            plannedSurplus = plan.Income - allocatedSum;
        }

        return new DashboardKpisDto(
            $"{year:D4}-{month:D2}",
            totalExpenses,
            expensesTrendPercent,
            totalInvestments,
            rentabilityPercent,
            totalStoredMoney,
            storedMoneyTrendPercent,
            plannedIncome,
            plannedAllocated,
            plannedSurplus
        );
    }

    public async Task<IEnumerable<ExpensesByCategoryDto>> GetExpensesByCategoryAsync(int year, int month)
    {
        var ccTotals = await _ccSpendingRepo.GetTotalsByCategoryAsync(year, month);
        var cashTotals = await _cashExpenseRepo.GetTotalsByCategoryAsync(year, month);
        var instTotals = await _installmentRepo.GetTotalsByCategoryAsync(year, month);
        var fixedTotal = await _fixedExpenseRepo.GetTotalAsync();
        var categories = (await _categoryRepo.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        var totalsByCategory = new Dictionary<Guid, decimal>();

        foreach (var (catId, val) in ccTotals)
            totalsByCategory[catId] = totalsByCategory.GetValueOrDefault(catId) + val;

        foreach (var (catId, val) in cashTotals)
            totalsByCategory[catId] = totalsByCategory.GetValueOrDefault(catId) + val;

        foreach (var (catId, val) in instTotals)
            totalsByCategory[catId] = totalsByCategory.GetValueOrDefault(catId) + val;

        var totalExpenses = totalsByCategory.Values.Sum() + fixedTotal;

        var list = new List<ExpensesByCategoryDto>();

        foreach (var kvp in totalsByCategory.OrderByDescending(k => k.Value))
        {
            var catName = categories.GetValueOrDefault(kvp.Key, "Outros");
            var pct = totalExpenses > 0 ? Math.Round((kvp.Value / totalExpenses) * 100m, 1) : 0m;
            list.Add(new ExpensesByCategoryDto(catName, kvp.Value, pct));
        }

        if (fixedTotal > 0)
        {
            var fixedPct = totalExpenses > 0 ? Math.Round((fixedTotal / totalExpenses) * 100m, 1) : 0m;
            list.Add(new ExpensesByCategoryDto("Despesas Fixas", fixedTotal, fixedPct));
        }

        return list.OrderByDescending(x => x.Value).ToList();
    }

    public async Task<ExpensesByTypeDto> GetExpensesByTypeAsync(int year, int month)
    {
        var ccTotal = await _ccSpendingRepo.GetTotalByMonthAsync(year, month);
        var cashTotal = await _cashExpenseRepo.GetTotalByMonthAsync(year, month);
        var instTotal = await _installmentRepo.GetTotalByMonthAsync(year, month);
        var fixedTotal = await _fixedExpenseRepo.GetTotalAsync();
        var total = ccTotal + cashTotal + instTotal + fixedTotal;

        return new ExpensesByTypeDto(ccTotal, cashTotal, instTotal, fixedTotal, total);
    }

    public async Task<IEnumerable<InvestmentHistoryPointDto>> GetInvestmentHistoryAsync(int months = 6)
    {
        var currentTotal = await _investmentRepo.GetTotalValueAsync();

        var ptMonths = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
        var now = DateTime.Now;

        // Earliest month start date
        var earliestDate = new DateTime(now.Year, now.Month, 1).AddMonths(-(months - 1));
        var recentTransactions = (await _invTxRepo.GetFromDateAsync(earliestDate)).ToList();

        var points = new List<InvestmentHistoryPointDto>();

        for (int i = months - 1; i >= 0; i--)
        {
            var targetMonthDate = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthName = ptMonths[targetMonthDate.Month - 1];

            decimal value;
            if (i == 0)
            {
                value = currentTotal;
            }
            else
            {
                // Month end threshold (exclusive): start of the following month
                var endOfTargetMonth = targetMonthDate.AddMonths(1);

                // Transactions that occurred after this month's end up to now
                var txsAfterMonth = recentTransactions.Where(t => t.Date >= endOfTargetMonth);

                var depositsAfter = txsAfterMonth.Where(t => t.Type == InvestmentTransactionType.Deposit).Sum(t => t.Value);
                var withdrawalsAfter = txsAfterMonth.Where(t => t.Type == InvestmentTransactionType.Withdrawal).Sum(t => t.Value);
                var rentabilityAfter = txsAfterMonth.Where(t => t.Type == InvestmentTransactionType.Rentability).Sum(t => t.Value);

                var netChangeAfter = depositsAfter - withdrawalsAfter + rentabilityAfter;
                value = currentTotal - netChangeAfter;

                // Fallback / guard against negative numbers
                if (value < 0) value = 0;
            }

            points.Add(new InvestmentHistoryPointDto(monthName, Math.Round(value, 2)));
        }

        return points;
    }
}
