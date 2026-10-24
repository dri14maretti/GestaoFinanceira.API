using System;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.PostgreSQL.Context;
using GerenciamentoFinanceiro.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciamentoFinanceiro.PostgreSQL;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgreSQL(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=gestaofinanceira;Username=postgres;Password=postgrespassword";

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseNpgsql(connectionString));

        // --- Scoped EF Core Repositories ---
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICreditCardRepository, CreditCardRepository>();
        services.AddScoped<ICreditCardSpendingRepository, CreditCardSpendingRepository>();
        services.AddScoped<ICashExpenseRepository, CashExpenseRepository>();
        services.AddScoped<IInstallmentRepository, InstallmentRepository>();
        services.AddScoped<IFixedExpenseRepository, FixedExpenseRepository>();
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();
        services.AddScoped<IInvestmentTransactionRepository, InvestmentTransactionRepository>();
        services.AddScoped<IInvestmentCategoryRepository, InvestmentCategoryRepository>();
        services.AddScoped<IStoredMoneyFundRepository, StoredMoneyFundRepository>();
        services.AddScoped<IFundTransactionRepository, FundTransactionRepository>();
        services.AddScoped<IMonthlyPlanRepository, MonthlyPlanRepository>();
        services.AddScoped<IPlanAllocationRepository, PlanAllocationRepository>();
        services.AddScoped<IPlanningCategoryRepository, PlanningCategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
