using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciamentoFinanceiro.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureFinanceSchemaKeysAndIndexes : Migration
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
                name: "IX_PlanAllocations_MonthlyPlanId",
                table: "PlanAllocations");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPlans_UserId",
                table: "MonthlyPlans");

            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "StoredMoneyFunds",
                newName: "StoredMoneyFunds",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshTokens",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "PlanningCategories",
                newName: "PlanningCategories",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "PlanAllocations",
                newName: "PlanAllocations",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "MonthlyPlans",
                newName: "MonthlyPlans",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "InvestmentTransactions",
                newName: "InvestmentTransactions",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "Investments",
                newName: "Investments",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "InvestmentCategories",
                newName: "InvestmentCategories",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "Installments",
                newName: "Installments",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "FundTransactions",
                newName: "FundTransactions",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "FixedExpenses",
                newName: "FixedExpenses",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "CreditCardSpendings",
                newName: "CreditCardSpendings",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "CreditCards",
                newName: "CreditCards",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "finance");

            migrationBuilder.RenameTable(
                name: "CashExpenses",
                newName: "CashExpenses",
                newSchema: "finance");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "StoredMoneyFunds",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "PlanningCategories",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "PlanAllocations",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "MonthlyPlans",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "InvestmentTransactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "Investments",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "InvestmentCategories",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "Installments",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "FundTransactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "FixedExpenses",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "CreditCardSpendings",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "CreditCards",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "finance",
                table: "CashExpenses",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                schema: "finance",
                table: "RefreshTokens",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentTransactions_UserId_Date",
                schema: "finance",
                table: "InvestmentTransactions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Installments_UserId_Month",
                schema: "finance",
                table: "Installments",
                columns: new[] { "UserId", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_FundTransactions_UserId_Date",
                schema: "finance",
                table: "FundTransactions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardSpendings_UserId_Date",
                schema: "finance",
                table: "CreditCardSpendings",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_Type",
                schema: "finance",
                table: "Categories",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_CashExpenses_UserId_Bank",
                schema: "finance",
                table: "CashExpenses",
                columns: new[] { "UserId", "Bank" });

            migrationBuilder.CreateIndex(
                name: "IX_CashExpenses_UserId_Date",
                schema: "finance",
                table: "CashExpenses",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                schema: "finance",
                table: "CashExpenses",
                column: "CategoryId",
                principalSchema: "finance",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                schema: "finance",
                table: "CreditCardSpendings",
                column: "CategoryId",
                principalSchema: "finance",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                schema: "finance",
                table: "CreditCardSpendings",
                column: "CreditCardId",
                principalSchema: "finance",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                schema: "finance",
                table: "FixedExpenses",
                column: "CreditCardId",
                principalSchema: "finance",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                schema: "finance",
                table: "Installments",
                column: "CategoryId",
                principalSchema: "finance",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                schema: "finance",
                table: "Installments",
                column: "CreditCardId",
                principalSchema: "finance",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                schema: "finance",
                table: "Investments",
                column: "InvestmentCategoryId",
                principalSchema: "finance",
                principalTable: "InvestmentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                schema: "finance",
                table: "PlanAllocations",
                column: "PlanningCategoryId",
                principalSchema: "finance",
                principalTable: "PlanningCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId",
                schema: "finance",
                table: "RefreshTokens",
                column: "ReplacedByTokenId",
                principalSchema: "finance",
                principalTable: "RefreshTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                schema: "finance",
                table: "CashExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_Categories_CategoryId",
                schema: "finance",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardSpendings_CreditCards_CreditCardId",
                schema: "finance",
                table: "CreditCardSpendings");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                schema: "finance",
                table: "FixedExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_Categories_CategoryId",
                schema: "finance",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Installments_CreditCards_CreditCardId",
                schema: "finance",
                table: "Installments");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                schema: "finance",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                schema: "finance",
                table: "PlanAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId",
                schema: "finance",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                schema: "finance",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentTransactions_UserId_Date",
                schema: "finance",
                table: "InvestmentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Installments_UserId_Month",
                schema: "finance",
                table: "Installments");

            migrationBuilder.DropIndex(
                name: "IX_FundTransactions_UserId_Date",
                schema: "finance",
                table: "FundTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardSpendings_UserId_Date",
                schema: "finance",
                table: "CreditCardSpendings");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId_Type",
                schema: "finance",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_CashExpenses_UserId_Bank",
                schema: "finance",
                table: "CashExpenses");

            migrationBuilder.DropIndex(
                name: "IX_CashExpenses_UserId_Date",
                schema: "finance",
                table: "CashExpenses");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "finance",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "StoredMoneyFunds",
                schema: "finance",
                newName: "StoredMoneyFunds");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "finance",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "PlanningCategories",
                schema: "finance",
                newName: "PlanningCategories");

            migrationBuilder.RenameTable(
                name: "PlanAllocations",
                schema: "finance",
                newName: "PlanAllocations");

            migrationBuilder.RenameTable(
                name: "MonthlyPlans",
                schema: "finance",
                newName: "MonthlyPlans");

            migrationBuilder.RenameTable(
                name: "InvestmentTransactions",
                schema: "finance",
                newName: "InvestmentTransactions");

            migrationBuilder.RenameTable(
                name: "Investments",
                schema: "finance",
                newName: "Investments");

            migrationBuilder.RenameTable(
                name: "InvestmentCategories",
                schema: "finance",
                newName: "InvestmentCategories");

            migrationBuilder.RenameTable(
                name: "Installments",
                schema: "finance",
                newName: "Installments");

            migrationBuilder.RenameTable(
                name: "FundTransactions",
                schema: "finance",
                newName: "FundTransactions");

            migrationBuilder.RenameTable(
                name: "FixedExpenses",
                schema: "finance",
                newName: "FixedExpenses");

            migrationBuilder.RenameTable(
                name: "CreditCardSpendings",
                schema: "finance",
                newName: "CreditCardSpendings");

            migrationBuilder.RenameTable(
                name: "CreditCards",
                schema: "finance",
                newName: "CreditCards");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "finance",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "CashExpenses",
                schema: "finance",
                newName: "CashExpenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "StoredMoneyFunds",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PlanningCategories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PlanAllocations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MonthlyPlans",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InvestmentTransactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Investments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InvestmentCategories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Installments",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FundTransactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FixedExpenses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CreditCardSpendings",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CreditCards",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Categories",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CashExpenses",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAllocations_MonthlyPlanId",
                table: "PlanAllocations",
                column: "MonthlyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPlans_UserId",
                table: "MonthlyPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashExpenses_Categories_CategoryId",
                table: "CashExpenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

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
                name: "FK_FixedExpenses_CreditCards_CreditCardId",
                table: "FixedExpenses",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");

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
                name: "FK_Investments_InvestmentCategories_InvestmentCategoryId",
                table: "Investments",
                column: "InvestmentCategoryId",
                principalTable: "InvestmentCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAllocations_PlanningCategories_PlanningCategoryId",
                table: "PlanAllocations",
                column: "PlanningCategoryId",
                principalTable: "PlanningCategories",
                principalColumn: "Id");
        }
    }
}
