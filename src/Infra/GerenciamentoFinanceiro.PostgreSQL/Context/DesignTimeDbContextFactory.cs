using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GerenciamentoFinanceiro.PostgreSQL.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=gestaofinanceira;Username=postgres;Password=postgrespassword";

        optionsBuilder.UseNpgsql(connectionString);

        return new FinanceDbContext(optionsBuilder.Options);
    }
}
