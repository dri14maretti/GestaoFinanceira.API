START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CashExpenses" DROP CONSTRAINT "FK_CashExpenses_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" DROP CONSTRAINT "FK_CreditCardSpendings_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" DROP CONSTRAINT "FK_CreditCardSpendings_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FixedExpenses" DROP CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" DROP CONSTRAINT "FK_Installments_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" DROP CONSTRAINT "FK_Installments_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Investments" DROP CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanAllocations" DROP CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    DROP INDEX "IX_MonthlyPlans_Month";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "StoredMoneyFunds" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanningCategories" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanAllocations" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "MonthlyPlans" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "InvestmentTransactions" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Investments" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "InvestmentCategories" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FundTransactions" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FixedExpenses" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCards" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Categories" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CashExpenses" ALTER COLUMN "CategoryId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CashExpenses" ADD "UserId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Email" character varying(256) NOT NULL,
        "PasswordHash" character varying(256) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    INSERT INTO "Users" ("Id", "Email", "PasswordHash", "CreatedAt")
    VALUES ('00000000-0000-0000-0000-000000000001', 'demo@gestaofinanceira.local', '$argon2id$v=19$m=19456,t=2,p=1$NnOhZMXuwHPLpQ5oVwPgUA==$om9s0+fBcFel77TDmWzcq2YPcy2negt36RgnDUQGufI=', TIMESTAMPTZ '2026-10-16T21:54:44.879053Z');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "Categories" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "CreditCards" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "CreditCardSpendings" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "CashExpenses" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "Installments" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "FixedExpenses" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "InvestmentCategories" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "Investments" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "InvestmentTransactions" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "StoredMoneyFunds" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "FundTransactions" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "PlanningCategories" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "MonthlyPlans" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    UPDATE "PlanAllocations" SET "UserId" = '00000000-0000-0000-0000-000000000001' WHERE "UserId" = '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE TABLE "RefreshTokens" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "TokenHash" character varying(128) NOT NULL,
        "FamilyId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "ExpiresAt" timestamp with time zone NOT NULL,
        "RevokedAt" timestamp with time zone,
        "ReplacedByTokenId" uuid,
        CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_StoredMoneyFunds_UserId" ON "StoredMoneyFunds" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_PlanningCategories_UserId" ON "PlanningCategories" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_PlanAllocations_UserId" ON "PlanAllocations" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_MonthlyPlans_UserId" ON "MonthlyPlans" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE UNIQUE INDEX "IX_MonthlyPlans_UserId_Month" ON "MonthlyPlans" ("UserId", "Month");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_InvestmentTransactions_UserId" ON "InvestmentTransactions" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_Investments_UserId" ON "Investments" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_InvestmentCategories_UserId" ON "InvestmentCategories" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_Installments_UserId" ON "Installments" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_FundTransactions_UserId" ON "FundTransactions" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_FixedExpenses_UserId" ON "FixedExpenses" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_CreditCardSpendings_UserId" ON "CreditCardSpendings" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_CreditCards_UserId" ON "CreditCards" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_Categories_UserId" ON "Categories" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_CashExpenses_UserId" ON "CashExpenses" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_RefreshTokens_FamilyId" ON "RefreshTokens" ("FamilyId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE UNIQUE INDEX "IX_RefreshTokens_TokenHash" ON "RefreshTokens" ("TokenHash");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CashExpenses" ADD CONSTRAINT "FK_CashExpenses_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CashExpenses" ADD CONSTRAINT "FK_CashExpenses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Categories" ADD CONSTRAINT "FK_Categories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCards" ADD CONSTRAINT "FK_CreditCards_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" ADD CONSTRAINT "FK_CreditCardSpendings_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" ADD CONSTRAINT "FK_CreditCardSpendings_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "CreditCardSpendings" ADD CONSTRAINT "FK_CreditCardSpendings_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FixedExpenses" ADD CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FixedExpenses" ADD CONSTRAINT "FK_FixedExpenses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "FundTransactions" ADD CONSTRAINT "FK_FundTransactions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" ADD CONSTRAINT "FK_Installments_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" ADD CONSTRAINT "FK_Installments_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Installments" ADD CONSTRAINT "FK_Installments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "InvestmentCategories" ADD CONSTRAINT "FK_InvestmentCategories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Investments" ADD CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId" FOREIGN KEY ("InvestmentCategoryId") REFERENCES "InvestmentCategories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "Investments" ADD CONSTRAINT "FK_Investments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "InvestmentTransactions" ADD CONSTRAINT "FK_InvestmentTransactions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "MonthlyPlans" ADD CONSTRAINT "FK_MonthlyPlans_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanAllocations" ADD CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId" FOREIGN KEY ("PlanningCategoryId") REFERENCES "PlanningCategories" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanAllocations" ADD CONSTRAINT "FK_PlanAllocations_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "PlanningCategories" ADD CONSTRAINT "FK_PlanningCategories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    ALTER TABLE "StoredMoneyFunds" ADD CONSTRAINT "FK_StoredMoneyFunds_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261016215230_AddAuthenticationAndUserIsolation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261016215230_AddAuthenticationAndUserIsolation', '10.0.13');
    END IF;
END $EF$;
COMMIT;

