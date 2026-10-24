CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Categories" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Type" character varying(50) NOT NULL,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
);

CREATE TABLE "CreditCards" (
    "Id" uuid NOT NULL,
    "BankName" character varying(100) NOT NULL,
    "ClosingDate" integer NOT NULL,
    "DueDate" integer NOT NULL,
    CONSTRAINT "PK_CreditCards" PRIMARY KEY ("Id")
);

CREATE TABLE "InvestmentCategories" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    CONSTRAINT "PK_InvestmentCategories" PRIMARY KEY ("Id")
);

CREATE TABLE "MonthlyPlans" (
    "Id" uuid NOT NULL,
    "Month" timestamp without time zone NOT NULL,
    "Income" numeric(18,2) NOT NULL,
    CONSTRAINT "PK_MonthlyPlans" PRIMARY KEY ("Id")
);

CREATE TABLE "PlanningCategories" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    CONSTRAINT "PK_PlanningCategories" PRIMARY KEY ("Id")
);

CREATE TABLE "StoredMoneyFunds" (
    "Id" uuid NOT NULL,
    "TotalValue" numeric(18,2) NOT NULL,
    "Description" character varying(250) NOT NULL,
    CONSTRAINT "PK_StoredMoneyFunds" PRIMARY KEY ("Id")
);

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

CREATE TABLE "FixedExpenses" (
    "Id" uuid NOT NULL,
    "Value" numeric(18,2) NOT NULL,
    "Description" character varying(250) NOT NULL,
    "CreditCardId" uuid NOT NULL,
    CONSTRAINT "PK_FixedExpenses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_FixedExpenses_CreditCards_CreditCardId" FOREIGN KEY ("CreditCardId") REFERENCES "CreditCards" ("Id") ON DELETE RESTRICT
);

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

CREATE TABLE "Investments" (
    "Id" uuid NOT NULL,
    "Value" numeric(18,2) NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "InvestmentCategoryId" uuid NOT NULL,
    CONSTRAINT "PK_Investments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Investments_InvestmentCategories_InvestmentCategoryId" FOREIGN KEY ("InvestmentCategoryId") REFERENCES "InvestmentCategories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "PlanAllocations" (
    "Id" uuid NOT NULL,
    "MonthlyPlanId" uuid NOT NULL,
    "PlanningCategoryId" uuid NOT NULL,
    "AllocatedValue" numeric(18,2) NOT NULL,
    CONSTRAINT "PK_PlanAllocations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PlanAllocations_MonthlyPlans_MonthlyPlanId" FOREIGN KEY ("MonthlyPlanId") REFERENCES "MonthlyPlans" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PlanAllocations_PlanningCategories_PlanningCategoryId" FOREIGN KEY ("PlanningCategoryId") REFERENCES "PlanningCategories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "FundTransactions" (
    "Id" uuid NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Value" numeric(18,2) NOT NULL,
    "FundId" uuid NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_FundTransactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_FundTransactions_StoredMoneyFunds_FundId" FOREIGN KEY ("FundId") REFERENCES "StoredMoneyFunds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "InvestmentTransactions" (
    "Id" uuid NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Value" numeric(18,2) NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "InvestmentId" uuid NOT NULL,
    CONSTRAINT "PK_InvestmentTransactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_InvestmentTransactions_Investments_InvestmentId" FOREIGN KEY ("InvestmentId") REFERENCES "Investments" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CashExpenses_Bank" ON "CashExpenses" ("Bank");

CREATE INDEX "IX_CashExpenses_CategoryId" ON "CashExpenses" ("CategoryId");

CREATE INDEX "IX_CashExpenses_Date" ON "CashExpenses" ("Date");

CREATE INDEX "IX_CreditCardSpendings_CategoryId" ON "CreditCardSpendings" ("CategoryId");

CREATE INDEX "IX_CreditCardSpendings_CreditCardId" ON "CreditCardSpendings" ("CreditCardId");

CREATE INDEX "IX_CreditCardSpendings_Date" ON "CreditCardSpendings" ("Date");

CREATE INDEX "IX_FixedExpenses_CreditCardId" ON "FixedExpenses" ("CreditCardId");

CREATE INDEX "IX_FundTransactions_Date" ON "FundTransactions" ("Date");

CREATE INDEX "IX_FundTransactions_FundId" ON "FundTransactions" ("FundId");

CREATE INDEX "IX_Installments_CategoryId" ON "Installments" ("CategoryId");

CREATE INDEX "IX_Installments_CreditCardId" ON "Installments" ("CreditCardId");

CREATE INDEX "IX_Installments_Month" ON "Installments" ("Month");

CREATE INDEX "IX_Investments_InvestmentCategoryId" ON "Investments" ("InvestmentCategoryId");

CREATE INDEX "IX_InvestmentTransactions_Date" ON "InvestmentTransactions" ("Date");

CREATE INDEX "IX_InvestmentTransactions_InvestmentId" ON "InvestmentTransactions" ("InvestmentId");

CREATE UNIQUE INDEX "IX_MonthlyPlans_Month" ON "MonthlyPlans" ("Month");

CREATE INDEX "IX_PlanAllocations_MonthlyPlanId" ON "PlanAllocations" ("MonthlyPlanId");

CREATE UNIQUE INDEX "IX_PlanAllocations_MonthlyPlanId_PlanningCategoryId" ON "PlanAllocations" ("MonthlyPlanId", "PlanningCategoryId");

CREATE INDEX "IX_PlanAllocations_PlanningCategoryId" ON "PlanAllocations" ("PlanningCategoryId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260919231559_InitialCreate', '10.0.12');

COMMIT;

