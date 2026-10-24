CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "Categories" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Type" character varying(50) NOT NULL,
        CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "CreditCards" (
        "Id" uuid NOT NULL,
        "BankName" character varying(100) NOT NULL,
        "ClosingDate" integer NOT NULL,
        "DueDate" integer NOT NULL,
        CONSTRAINT "PK_CreditCards" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "InvestmentCategories" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        CONSTRAINT "PK_InvestmentCategories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "MonthlyPlans" (
        "Id" uuid NOT NULL,
        "Month" timestamp without time zone NOT NULL,
        "Income" numeric(18,2) NOT NULL,
        CONSTRAINT "PK_MonthlyPlans" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "PlanningCategories" (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        CONSTRAINT "PK_PlanningCategories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "StoredMoneyFunds" (
        "Id" uuid NOT NULL,
        "TotalValue" numeric(18,2) NOT NULL,
        "Description" character varying(250) NOT NULL,
        CONSTRAINT "PK_StoredMoneyFunds" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "CashExpenses" (
        "Id" uuid NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "CategoryId" uuid NOT NULL,
        "Bank" character varying(100) NOT NULL,
        "Date" timestamp without time zone NOT NULL,
        "Description" character varying(250) NOT NULL,
        CONSTRAINT "PK_CashExpenses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CashExpenses_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "CreditCardSpendings" (
        "Id" uuid NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "CategoryId" uuid NOT NULL,
        "CreditCardId" uuid NOT NULL,
        "Date" timestamp without time zone NOT NULL,
        "Description" character varying(250) NOT NULL,
        CONSTRAINT "PK_CreditCardSpendings" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CreditCardSpendings_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_CreditCardSpendings_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "FixedExpenses" (
        "Id" uuid NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "Description" character varying(250) NOT NULL,
        "CreditCardId" uuid NOT NULL,
        CONSTRAINT "PK_FixedExpenses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "Installments" (
        "Id" uuid NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "CategoryId" uuid NOT NULL,
        "CreditCardId" uuid NOT NULL,
        "Month" timestamp without time zone NOT NULL,
        "Description" character varying(250) NOT NULL,
        "TotalInstallments" integer NOT NULL,
        "CurrentInstallment" integer NOT NULL,
        CONSTRAINT "PK_Installments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Installments_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Installments_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "Investments" (
        "Id" uuid NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "Date" timestamp without time zone NOT NULL,
        "InvestmentCategoryId" uuid NOT NULL,
        CONSTRAINT "PK_Investments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId" FOREIGN KEY ("InvestmentCategoryId") REFERENCES "InvestmentCategories" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "PlanAllocations" (
        "Id" uuid NOT NULL,
        "MonthlyPlanId" uuid NOT NULL,
        "PlanningCategoryId" uuid NOT NULL,
        "AllocatedValue" numeric(18,2) NOT NULL,
        CONSTRAINT "PK_PlanAllocations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_PlanAllocations_MonthlyPlans_MonthlyPlanId" FOREIGN KEY ("MonthlyPlanId") REFERENCES "MonthlyPlans" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId" FOREIGN KEY ("PlanningCategoryId") REFERENCES "PlanningCategories" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "FundTransactions" (
        "Id" uuid NOT NULL,
        "Type" character varying(50) NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "FundId" uuid NOT NULL,
        "Date" timestamp without time zone NOT NULL,
        CONSTRAINT "PK_FundTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_FundTransactions_StoredMoneyFunds_FundId" FOREIGN KEY ("FundId") REFERENCES "StoredMoneyFunds" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE TABLE "InvestmentTransactions" (
        "Id" uuid NOT NULL,
        "Type" character varying(50) NOT NULL,
        "Value" numeric(18,2) NOT NULL,
        "Date" timestamp without time zone NOT NULL,
        "InvestmentId" uuid NOT NULL,
        CONSTRAINT "PK_InvestmentTransactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_InvestmentTransactions_Investments_InvestmentId" FOREIGN KEY ("InvestmentId") REFERENCES "Investments" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CashExpenses_Bank" ON "CashExpenses" ("Bank");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CashExpenses_CategoryId" ON "CashExpenses" ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CashExpenses_Date" ON "CashExpenses" ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CreditCardSpendings_CategoryId" ON "CreditCardSpendings" ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CreditCardSpendings_CreditCardId" ON "CreditCardSpendings" ("CreditCardId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_CreditCardSpendings_Date" ON "CreditCardSpendings" ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_FixedExpenses_CreditCardId" ON "FixedExpenses" ("CreditCardId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_FundTransactions_Date" ON "FundTransactions" ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_FundTransactions_FundId" ON "FundTransactions" ("FundId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_Installments_CategoryId" ON "Installments" ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_Installments_CreditCardId" ON "Installments" ("CreditCardId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_Installments_Month" ON "Installments" ("Month");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_Investments_InvestmentCategoryId" ON "Investments" ("InvestmentCategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_InvestmentTransactions_Date" ON "InvestmentTransactions" ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_InvestmentTransactions_InvestmentId" ON "InvestmentTransactions" ("InvestmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_MonthlyPlans_Month" ON "MonthlyPlans" ("Month");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_PlanAllocations_MonthlyPlanId" ON "PlanAllocations" ("MonthlyPlanId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_PlanAllocations_MonthlyPlanId_PlanningCategoryId" ON "PlanAllocations" ("MonthlyPlanId", "PlanningCategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    CREATE INDEX "IX_PlanAllocations_PlanningCategoryId" ON "PlanAllocations" ("PlanningCategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260919231559_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260919231559_InitialCreate', '10.0.13');
    END IF;
END $EF$;
COMMIT;

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
    VALUES ('00000000-0000-0000-0000-000000000001', 'demo@gestaofinanceira.local', '$argon2id$v=19$m=19456,t=2,p=1$NnOhZMXuwHPLpQ5oVwPgUA==$om9s0+fBcFel77TDmWzcq2YPcy2negt36RgnDUQGufI=', TIMESTAMPTZ '2026-10-24T00:19:39.862731Z');
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

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CashExpenses" DROP CONSTRAINT "FK_CashExpenses_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CreditCardSpendings" DROP CONSTRAINT "FK_CreditCardSpendings_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CreditCardSpendings" DROP CONSTRAINT "FK_CreditCardSpendings_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "FixedExpenses" DROP CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Installments" DROP CONSTRAINT "FK_Installments_Categories_CategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Installments" DROP CONSTRAINT "FK_Installments_CreditCards_CreditCardId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Investments" DROP CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "PlanAllocations" DROP CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    DROP INDEX "IX_PlanAllocations_MonthlyPlanId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    DROP INDEX "IX_MonthlyPlans_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'finance') THEN
            CREATE SCHEMA finance;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Users" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "StoredMoneyFunds" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "RefreshTokens" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "PlanningCategories" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "PlanAllocations" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "MonthlyPlans" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "InvestmentTransactions" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Investments" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "InvestmentCategories" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Installments" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "FundTransactions" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "FixedExpenses" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CreditCardSpendings" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CreditCards" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "Categories" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE "CashExpenses" SET SCHEMA finance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Users" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."StoredMoneyFunds" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."RefreshTokens" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."PlanningCategories" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."PlanAllocations" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."MonthlyPlans" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."InvestmentTransactions" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Investments" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."InvestmentCategories" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Installments" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."FundTransactions" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."FixedExpenses" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CreditCardSpendings" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CreditCards" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Categories" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CashExpenses" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_RefreshTokens_ReplacedByTokenId" ON finance."RefreshTokens" ("ReplacedByTokenId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_InvestmentTransactions_UserId_Date" ON finance."InvestmentTransactions" ("UserId", "Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_Installments_UserId_Month" ON finance."Installments" ("UserId", "Month");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_FundTransactions_UserId_Date" ON finance."FundTransactions" ("UserId", "Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_CreditCardSpendings_UserId_Date" ON finance."CreditCardSpendings" ("UserId", "Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_Categories_UserId_Type" ON finance."Categories" ("UserId", "Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_CashExpenses_UserId_Bank" ON finance."CashExpenses" ("UserId", "Bank");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    CREATE INDEX "IX_CashExpenses_UserId_Date" ON finance."CashExpenses" ("UserId", "Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CashExpenses" ADD CONSTRAINT "FK_CashExpenses_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES finance."Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CreditCardSpendings" ADD CONSTRAINT "FK_CreditCardSpendings_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES finance."Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."CreditCardSpendings" ADD CONSTRAINT "FK_CreditCardSpendings_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES finance."CreditCards" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."FixedExpenses" ADD CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES finance."CreditCards" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Installments" ADD CONSTRAINT "FK_Installments_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES finance."Categories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Installments" ADD CONSTRAINT "FK_Installments_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES finance."CreditCards" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."Investments" ADD CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId" FOREIGN KEY ("InvestmentCategoryId") REFERENCES finance."InvestmentCategories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."PlanAllocations" ADD CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId" FOREIGN KEY ("PlanningCategoryId") REFERENCES finance."PlanningCategories" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    ALTER TABLE finance."RefreshTokens" ADD CONSTRAINT "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId" FOREIGN KEY ("ReplacedByTokenId") REFERENCES finance."RefreshTokens" ("Id") ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261024001629_ConfigureFinanceSchemaKeysAndIndexes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261024001629_ConfigureFinanceSchemaKeysAndIndexes', '10.0.13');
    END IF;
END $EF$;
COMMIT;

