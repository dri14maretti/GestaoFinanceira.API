using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GerenciamentoFinanceiro.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ExpenseService>();
            services.AddScoped<InvestmentService>();
            services.AddScoped<StoredMoneyService>();
            services.AddScoped<PlanningService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<AuthService>();
        }
    }
}
