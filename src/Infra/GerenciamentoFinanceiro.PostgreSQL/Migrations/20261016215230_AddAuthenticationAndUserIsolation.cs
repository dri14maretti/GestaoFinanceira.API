using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoFinanceiro.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationAndUserIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                table: "CashExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                table: "FixedExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                table: "PlanAllocations");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPlans_Month",
                table: "MonthlyPlans");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "StoredMoneyFunds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PlanningCategories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PlanAllocations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "MonthlyPlans",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "InvestmentTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Investments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "InvestmentCategories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Installments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FundTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FixedExpenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CreditCardSpendings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CreditCards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "CashExpenses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CashExpenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "CreatedAt" },
                values: new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    "demo@gestaofinanceira.local",
                    "$argon2id$v=19$m=19456,t=2,p=1$NnOhZMXuwHPLpQ5oVwPgUA==$om9s0+fBcFel77TDmWzcq2YPcy2negt36RgnDUQGufI=",
                    DateTime.UtcNow
                });

            var demoUserId = "00000000-0000-0000-0000-000000000001";
            migrationBuilder.Sql($"UPDATE \"Categories\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"CreditCards\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"CreditCardSpendings\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"CashExpenses\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"Installments\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"FixedExpenses\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"InvestmentCategories\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"Investments\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"InvestmentTransactions\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"StoredMoneyFunds\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"FundTransactions\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"PlanningCategories\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"MonthlyPlans\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");
            migrationBuilder.Sql($"UPDATE \"PlanAllocations\" SET \"UserId\" = '{demoUserId}' WHERE \"UserId\" = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FamilyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoredMoneyFunds_UserId",
                table: "StoredMoneyFunds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningCategories_UserId",
                table: "PlanningCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAllocations_UserId",
                table: "PlanAllocations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPlans_UserId",
                table: "MonthlyPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPlans_UserId_Month",
                table: "MonthlyPlans",
                columns: new[] { "UserId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_UserId",
                table: "InvestmentTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_UserId",
                table: "Investments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentCategories_UserId",
                table: "InvestmentCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Installments_UserId",
                table: "Installments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FundTransactions_UserId",
                table: "FundTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedExpenses_UserId",
                table: "FixedExpenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardSpendings_UserId",
                table: "CreditCardSpendings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashExpenses_UserId",
                table: "CashExpenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FamilyId",
                table: "RefreshTokens",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                table: "CashExpenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CashExpenses_Users_UserId",
                table: "CashExpenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Users_UserId",
                table: "CreditCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                table: "CreditCardSpendings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                table: "CreditCardSpendings",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_Users_UserId",
                table: "CreditCardSpendings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                table: "FixedExpenses",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedExpenses_Users_UserId",
                table: "FixedExpenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundTransactions_Users_UserId",
                table: "FundTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                table: "Installments",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                table: "Installments",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Users_UserId",
                table: "Installments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentCategories_Users_UserId",
                table: "InvestmentCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                table: "Investments",
                column: "InvestmentCategoryId",
                principalTable: "InvestmentCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Users_UserId",
                table: "Investments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentTransactions_Users_UserId",
                table: "InvestmentTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyPlans_Users_UserId",
                table: "MonthlyPlans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                table: "PlanAllocations",
                column: "PlanningCategoryId",
                principalTable: "PlanningCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAllocations_Users_UserId",
                table: "PlanAllocations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanningCategories_Users_UserId",
                table: "PlanningCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoredMoneyFunds_Users_UserId",
                table: "StoredMoneyFunds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                table: "CashExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_CashExpenses_Users_UserId",
                table: "CashExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Users_UserId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_Users_UserId",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                table: "FixedExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedExpenses_Users_UserId",
                table: "FixedExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_FundTransactions_Users_UserId",
                table: "FundTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Users_UserId",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentCategories_Users_UserId",
                table: "InvestmentCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Users_UserId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentTransactions_Users_UserId",
                table: "InvestmentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyPlans_Users_UserId",
                table: "MonthlyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                table: "PlanAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanAllocations_Users_UserId",
                table: "PlanAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanningCategories_Users_UserId",
                table: "PlanningCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_StoredMoneyFunds_Users_UserId",
                table: "StoredMoneyFunds");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_StoredMoneyFunds_UserId",
                table: "StoredMoneyFunds");

            migrationBuilder.DropIndex(
                name: "IX_PlanningCategories_UserId",
                table: "PlanningCategories");

            migrationBuilder.DropIndex(
                name: "IX_PlanAllocations_UserId",
                table: "PlanAllocations");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPlans_UserId",
                table: "MonthlyPlans");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPlans_UserId_Month",
                table: "MonthlyPlans");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentTransactions_UserId",
                table: "InvestmentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Investments_UserId",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentCategories_UserId",
                table: "InvestmentCategories");

            migrationBuilder.DropIndex(
                name: "IX_Installments_UserId",
                table: "Installments");

            migrationBuilder.DropIndex(
                name: "IX_FundTransactions_UserId",
                table: "FundTransactions");

            migrationBuilder.DropIndex(
                name: "IX_FixedExpenses_UserId",
                table: "FixedExpenses");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardSpendings_UserId",
                table: "CreditCardSpendings");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_CashExpenses_UserId",
                table: "CashExpenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StoredMoneyFunds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlanningCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlanAllocations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MonthlyPlans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InvestmentTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InvestmentCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Installments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FundTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FixedExpenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CreditCardSpendings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CashExpenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "CashExpenses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPlans_Month",
                table: "MonthlyPlans",
                column: "Month",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                table: "CashExpenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                table: "CreditCardSpendings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                table: "CreditCardSpendings",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                table: "FixedExpenses",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                table: "Installments",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                table: "Installments",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                table: "Investments",
                column: "InvestmentCategoryId",
                principalTable: "InvestmentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                table: "PlanAllocations",
                column: "PlanningCategoryId",
                principalTable: "PlanningCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
