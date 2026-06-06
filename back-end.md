I want you to plan the execution of the back-end part of this implementation plan. Please mock some data to come from the "database", as it will be implemented in a later stage of this project:

# Technical Requirements Document: Finance Management System

This document specifies the architecture and functional requirements for a Finance Management System, optimized for processing by an AI coding agent.

---

## 1. System Architecture

* **Backend:** ASP.NET Core (Web API)
* **Database:** PostgreSQL
* **Frontend:** React.js with Tailwind CSS (Selected for high compatibility with ASP.NET APIs)
* **Architecture Pattern:** Repository Pattern with Clean Architecture

---

## 2. Database Schema (PostgreSQL)

### 2.1 Expenses Module

* **Table: `CreditCardSpendings**` (Value, CategoryId, CreditCardId, Date, Description)
* **Table: `CashExpenses**` (PIX/Boleto) (Value, CategoryId, Bank, Date, Description)
* **Table: `Installments**` (Parcelas) (Value, CategoryId, CreditCardId, Month, Description)
* **Table: `FixedExpenses**` (Value, Description, CreditCardId)
* **Table: `Categories**` (Name, Type [Expense/Investment])
* **Table: `CreditCards**` (BankName, ClosingDate, DueDate)

### 2.2 Investments Module

* **Table: `Investments**` (Value, Date, InvestmentCategoryId)
* **Table: `InvestmentTransactions**` (Type [Deposit/Withdrawal/Rentability], Value, Date)
* **Table: `InvestmentCategories**` (Name)

### 2.3 Stored Money Module

* **Table: `StoredMoneyFunds**` (TotalValue, Description)
* **Table: `FundTransactions**` (Type [Add/Withdraw], Value, FundId, Date)

---

## 3. Functional Requirements

### 3.1 Expenses Management

* **CRUD Operations:**
* Credit Card Spendings (Tracked by value, category, issuing bank, date, and description).
* PIX / Boleto Expenses (Tracked by value, category, bank, date, and description). - Those transactions should be added up, separated by bank, so I can know exactly my balance on a given moment
* Installments (Manage monthly recurring "Parcelas" with value, category, card, and month).
* Spending Categories management.
* Credit Card entity management.
* Fixed Expenses management (Linked to a Credit Card).


* **Reporting:**
* **Segregated Spending Report:** Breakdown of expenses by Credit Card, PIX/Boleto, and Installments, further filtered by Category.
* **Monthly Expense Report:** Comprehensive tracking of all expenses within a specific month, grouped by category.



### 3.2 Investments Management

* **Tracking:**
* Manage core investments (Value, Date, Category).
* Investment Category management.


* **Operations:**
* Withdrawal functionality.
* Manual Rentability Updates: User-triggered updates to reflect investment growth/earnings.


* **Reporting:**
* **Annual Investment Report:** Summary of portfolio performance and totals over a calendar year.



### 3.3 Stored Money (Savings Funds)

* **Management:**
* Create and manage specific funds for targeted savings (e.g., "Emergency Fund", "Travel").
* Fields: Total Value, Description.


* **Transactions:**
* Logic to add (deposit) money to specific funds.
* Logic to withdraw money from specific funds.

### 3.4 Monthly planning functionality
* System should be able to create a spending plan, where I can register my income and plan each part of that spending depending on the planning categories
* Manage Planning Categories, that should be different from the other categories

---

## 4. Technical Constraints

* Ensure all reports utilize optimized SQL queries for PostgreSQL.
* The ASP.NET backend must implement RESTful endpoints for all CRUD operations.
* Frontend must provide a responsive dashboard to visualize the segregated reports.