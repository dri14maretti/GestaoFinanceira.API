using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciamentoFinanceiro.PostgreSQL
{
    public static class DependencyInjection
    {
        public static void AddPostgreSQL(this IServiceCollection services)
        {
            // --- Dependency Injection: Mock Repositories (Singletons because they use static data) ---
            services.AddSingleton<ICategoryRepository, MockCategoryRepository>();
            services.AddSingleton<ICreditCardRepository, MockCreditCardRepository>();
            services.AddSingleton<ICreditCardSpendingRepository, MockCreditCardSpendingRepository>();
            services.AddSingleton<ICashExpenseRepository, MockCashExpenseRepository>();
            services.AddSingleton<IInstallmentRepository, MockInstallmentRepository>();
            services.AddSingleton<IFixedExpenseRepository, MockFixedExpenseRepository>();
            services.AddSingleton<IInvestmentRepository, MockInvestmentRepository>();
            services.AddSingleton<IInvestmentTransactionRepository, MockInvestmentTransactionRepository>();
            services.AddSingleton<IInvestmentCategoryRepository, MockInvestmentCategoryRepository>();
            services.AddSingleton<IStoredMoneyFundRepository, MockStoredMoneyFundRepository>();
            services.AddSingleton<IFundTransactionRepository, MockFundTransactionRepository>();
            services.AddSingleton<IMonthlyPlanRepository, MockMonthlyPlanRepository>();
            services.AddSingleton<IPlanAllocationRepository, MockPlanAllocationRepository>();
            services.AddSingleton<IPlanningCategoryRepository, MockPlanningCategoryRepository>();
        }

    }
}
