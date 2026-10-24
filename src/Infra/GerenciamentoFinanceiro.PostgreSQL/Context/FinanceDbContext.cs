using System;
using System.Threading;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoFinanceiro.PostgreSQL.Context;

public class FinanceDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public FinanceDbContext(
        DbContextOptions<FinanceDbContext> options,
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService ?? new NullCurrentUserService();
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<CreditCardSpending> CreditCardSpendings => Set<CreditCardSpending>();
    public DbSet<CashExpense> CashExpenses => Set<CashExpense>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<FixedExpense> FixedExpenses => Set<FixedExpense>();
    public DbSet<InvestmentCategory> InvestmentCategories => Set<InvestmentCategory>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<InvestmentTransaction> InvestmentTransactions => Set<InvestmentTransaction>();
    public DbSet<StoredMoneyFund> StoredMoneyFunds => Set<StoredMoneyFund>();
    public DbSet<FundTransaction> FundTransactions => Set<FundTransaction>();
    public DbSet<PlanningCategory> PlanningCategories => Set<PlanningCategory>();
    public DbSet<MonthlyPlan> MonthlyPlans => Set<MonthlyPlan>();
    public DbSet<PlanAllocation> PlanAllocations => Set<PlanAllocation>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IUserOwnedEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.UserId == Guid.Empty)
                {
                    var currentUserId = _currentUserService.UserId;
                    if (!currentUserId.HasValue || currentUserId.Value == Guid.Empty)
                    {
                        throw new UnauthorizedAccessException("Cannot persist a tenant-owned entity without an authenticated user context.");
                    }
                    entry.Entity.AssignOwner(currentUserId.Value);
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("finance");

        // --- User ---
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(u => u.Email).IsRequired().HasMaxLength(256);
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
            b.Property(u => u.CreatedAt).HasColumnType("timestamp with time zone");
        });

        // --- RefreshToken ---
        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshTokens");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(r => r.UserId).IsRequired();
            b.Property(r => r.TokenHash).IsRequired().HasMaxLength(128);
            b.HasIndex(r => r.TokenHash).IsUnique();
            b.HasIndex(r => r.UserId);
            b.HasIndex(r => r.FamilyId);
            b.HasIndex(r => r.ReplacedByTokenId);
            b.Property(r => r.CreatedAt).HasColumnType("timestamp with time zone");
            b.Property(r => r.ExpiresAt).HasColumnType("timestamp with time zone");
            b.Property(r => r.RevokedAt).HasColumnType("timestamp with time zone");

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<RefreshToken>()
                .WithMany()
                .HasForeignKey(r => r.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Category ---
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("Categories");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.Property(c => c.Type).HasConversion<string>().HasMaxLength(50);

            b.Property(c => c.UserId).IsRequired();
            b.HasIndex(c => c.UserId);
            b.HasIndex(c => new { c.UserId, c.Type });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(c => _currentUserService.UserId != null && c.UserId == _currentUserService.UserId.Value);
        });

        // --- CreditCard ---
        modelBuilder.Entity<CreditCard>(b =>
        {
            b.ToTable("CreditCards");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(c => c.BankName).IsRequired().HasMaxLength(100);
            b.Property(c => c.ClosingDate).IsRequired();
            b.Property(c => c.DueDate).IsRequired();

            b.Property(c => c.UserId).IsRequired();
            b.HasIndex(c => c.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(c => _currentUserService.UserId != null && c.UserId == _currentUserService.UserId.Value);
        });

        // --- CreditCardSpending ---
        modelBuilder.Entity<CreditCardSpending>(b =>
        {
            b.ToTable("CreditCardSpendings");
            b.HasKey(s => s.Id);
            b.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(s => s.Value).HasPrecision(18, 2);
            b.Property(s => s.Description).HasMaxLength(250);
            b.Property(s => s.Date).HasColumnType("timestamp without time zone");

            b.Property(s => s.UserId).IsRequired();
            b.HasIndex(s => s.UserId);
            b.HasIndex(s => new { s.UserId, s.Date });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<CreditCard>()
                .WithMany()
                .HasForeignKey(s => s.CreditCardId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(s => s.Date);
            b.HasIndex(s => s.CategoryId);
            b.HasIndex(s => s.CreditCardId);

            b.HasQueryFilter(s => _currentUserService.UserId != null && s.UserId == _currentUserService.UserId.Value);
        });

        // --- CashExpense ---
        modelBuilder.Entity<CashExpense>(b =>
        {
            b.ToTable("CashExpenses");
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(e => e.Value).HasPrecision(18, 2);
            b.Property(e => e.Bank).IsRequired().HasMaxLength(100);
            b.Property(e => e.Description).HasMaxLength(250);
            b.Property(e => e.Date).HasColumnType("timestamp without time zone");

            b.Property(e => e.UserId).IsRequired();
            b.HasIndex(e => e.UserId);
            b.HasIndex(e => new { e.UserId, e.Date });
            b.HasIndex(e => new { e.UserId, e.Bank });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(e => e.Date);
            b.HasIndex(e => e.CategoryId);
            b.HasIndex(e => e.Bank);

            b.HasQueryFilter(e => _currentUserService.UserId != null && e.UserId == _currentUserService.UserId.Value);
        });

        // --- Installment ---
        modelBuilder.Entity<Installment>(b =>
        {
            b.ToTable("Installments");
            b.HasKey(i => i.Id);
            b.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(i => i.Value).HasPrecision(18, 2);
            b.Property(i => i.Description).HasMaxLength(250);
            b.Property(i => i.Month).HasColumnType("timestamp without time zone");

            b.Property(i => i.UserId).IsRequired();
            b.HasIndex(i => i.UserId);
            b.HasIndex(i => new { i.UserId, i.Month });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<CreditCard>()
                .WithMany()
                .HasForeignKey(i => i.CreditCardId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(i => i.Month);
            b.HasIndex(i => i.CreditCardId);
            b.HasIndex(i => i.CategoryId);

            b.HasQueryFilter(i => _currentUserService.UserId != null && i.UserId == _currentUserService.UserId.Value);
        });

        // --- FixedExpense ---
        modelBuilder.Entity<FixedExpense>(b =>
        {
            b.ToTable("FixedExpenses");
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(f => f.Value).HasPrecision(18, 2);
            b.Property(f => f.Description).IsRequired().HasMaxLength(250);

            b.Property(f => f.UserId).IsRequired();
            b.HasIndex(f => f.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<CreditCard>()
                .WithMany()
                .HasForeignKey(f => f.CreditCardId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(f => f.CreditCardId);

            b.HasQueryFilter(f => _currentUserService.UserId != null && f.UserId == _currentUserService.UserId.Value);
        });

        // --- InvestmentCategory ---
        modelBuilder.Entity<InvestmentCategory>(b =>
        {
            b.ToTable("InvestmentCategories");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);

            b.Property(c => c.UserId).IsRequired();
            b.HasIndex(c => c.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(c => _currentUserService.UserId != null && c.UserId == _currentUserService.UserId.Value);
        });

        // --- Investment ---
        modelBuilder.Entity<Investment>(b =>
        {
            b.ToTable("Investments");
            b.HasKey(i => i.Id);
            b.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(i => i.Value).HasPrecision(18, 2);
            b.Property(i => i.Date).HasColumnType("timestamp without time zone");

            b.Property(i => i.UserId).IsRequired();
            b.HasIndex(i => i.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<InvestmentCategory>()
                .WithMany()
                .HasForeignKey(i => i.InvestmentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(i => i.InvestmentCategoryId);

            b.HasQueryFilter(i => _currentUserService.UserId != null && i.UserId == _currentUserService.UserId.Value);
        });

        // --- InvestmentTransaction ---
        modelBuilder.Entity<InvestmentTransaction>(b =>
        {
            b.ToTable("InvestmentTransactions");
            b.HasKey(t => t.Id);
            b.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(t => t.Type).HasConversion<string>().HasMaxLength(50);
            b.Property(t => t.Value).HasPrecision(18, 2);
            b.Property(t => t.Date).HasColumnType("timestamp without time zone");

            b.Property(t => t.UserId).IsRequired();
            b.HasIndex(t => t.UserId);
            b.HasIndex(t => new { t.UserId, t.Date });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Investment>()
                .WithMany()
                .HasForeignKey(t => t.InvestmentId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(t => t.InvestmentId);
            b.HasIndex(t => t.Date);

            b.HasQueryFilter(t => _currentUserService.UserId != null && t.UserId == _currentUserService.UserId.Value);
        });

        // --- StoredMoneyFund ---
        modelBuilder.Entity<StoredMoneyFund>(b =>
        {
            b.ToTable("StoredMoneyFunds");
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(f => f.TotalValue).HasPrecision(18, 2);
            b.Property(f => f.Description).IsRequired().HasMaxLength(250);

            b.Property(f => f.UserId).IsRequired();
            b.HasIndex(f => f.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(f => _currentUserService.UserId != null && f.UserId == _currentUserService.UserId.Value);
        });

        // --- FundTransaction ---
        modelBuilder.Entity<FundTransaction>(b =>
        {
            b.ToTable("FundTransactions");
            b.HasKey(t => t.Id);
            b.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(t => t.Type).HasConversion<string>().HasMaxLength(50);
            b.Property(t => t.Value).HasPrecision(18, 2);
            b.Property(t => t.Date).HasColumnType("timestamp without time zone");

            b.Property(t => t.UserId).IsRequired();
            b.HasIndex(t => t.UserId);
            b.HasIndex(t => new { t.UserId, t.Date });
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<StoredMoneyFund>()
                .WithMany()
                .HasForeignKey(t => t.FundId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(t => t.FundId);
            b.HasIndex(t => t.Date);

            b.HasQueryFilter(t => _currentUserService.UserId != null && t.UserId == _currentUserService.UserId.Value);
        });

        // --- PlanningCategory ---
        modelBuilder.Entity<PlanningCategory>(b =>
        {
            b.ToTable("PlanningCategories");
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(p => p.Name).IsRequired().HasMaxLength(100);

            b.Property(p => p.UserId).IsRequired();
            b.HasIndex(p => p.UserId);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(p => _currentUserService.UserId != null && p.UserId == _currentUserService.UserId.Value);
        });

        // --- MonthlyPlan ---
        modelBuilder.Entity<MonthlyPlan>(b =>
        {
            b.ToTable("MonthlyPlans");
            b.HasKey(p => p.Id);
            b.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(p => p.Month).HasColumnType("timestamp without time zone");
            b.Property(p => p.Income).HasPrecision(18, 2);

            b.Property(p => p.UserId).IsRequired();
            b.HasIndex(p => new { p.UserId, p.Month }).IsUnique();

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(p => _currentUserService.UserId != null && p.UserId == _currentUserService.UserId.Value);
        });

        // --- PlanAllocation ---
        modelBuilder.Entity<PlanAllocation>(b =>
        {
            b.ToTable("PlanAllocations");
            b.HasKey(a => a.Id);
            b.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
            b.Property(a => a.AllocatedValue).HasPrecision(18, 2);

            b.Property(a => a.UserId).IsRequired();
            b.HasIndex(a => a.UserId);
            b.HasIndex(a => new { a.MonthlyPlanId, a.PlanningCategoryId }).IsUnique();

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<MonthlyPlan>()
                .WithMany()
                .HasForeignKey(a => a.MonthlyPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<PlanningCategory>()
                .WithMany()
                .HasForeignKey(a => a.PlanningCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(a => _currentUserService.UserId != null && a.UserId == _currentUserService.UserId.Value);
        });
    }

    private class NullCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => null;
        public Guid GetRequiredUserId() => throw new UnauthorizedAccessException("No authenticated user context available.");
    }
}
