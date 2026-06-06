using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.Application.DTOs.StoredMoney;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.Application.Services;

public class StoredMoneyService
{
    private readonly IStoredMoneyFundRepository _fundRepo;
    private readonly IFundTransactionRepository _transactionRepo;

    public StoredMoneyService(IStoredMoneyFundRepository fundRepo, IFundTransactionRepository transactionRepo)
    {
        _fundRepo = fundRepo;
        _transactionRepo = transactionRepo;
    }

    // --- Funds ---

    public async Task<IEnumerable<StoredMoneyFundDto>> GetAllFundsAsync()
    {
        var funds = await _fundRepo.GetAllAsync();
        return funds.Select(f => new StoredMoneyFundDto(f.Id, f.TotalValue, f.Description));
    }

    public async Task<StoredMoneyFundDto?> GetFundByIdAsync(Guid id)
    {
        var entity = await _fundRepo.GetByIdAsync(id);
        return entity == null ? null : new StoredMoneyFundDto(entity.Id, entity.TotalValue, entity.Description);
    }

    public async Task<StoredMoneyFundDto> CreateFundAsync(CreateStoredMoneyFundDto dto)
    {
        var entity = new StoredMoneyFund(dto.TotalValue, dto.Description);
        await _fundRepo.AddAsync(entity);
        return new StoredMoneyFundDto(entity.Id, entity.TotalValue, entity.Description);
    }

    public async Task<StoredMoneyFundDto?> UpdateFundAsync(Guid id, CreateStoredMoneyFundDto dto)
    {
        var fund = await _fundRepo.GetByIdAsync(id);
        if (fund == null) return null;
        fund.UpdateDescription(dto.Description);
        await _fundRepo.UpdateAsync(fund);
        return new StoredMoneyFundDto(fund.Id, fund.TotalValue, fund.Description);
    }

    public async Task<bool> DeleteFundAsync(Guid id) => await _fundRepo.DeleteAsync(id);

    // --- Transactions ---

    public async Task<IEnumerable<FundTransactionDto>> GetTransactionsByFundAsync(Guid fundId)
    {
        var transactions = await _transactionRepo.GetByFundIdAsync(fundId);
        return transactions.Select(MapToTransactionDto);
    }

    public async Task<FundTransactionDto> CreateTransactionAsync(CreateFundTransactionDto dto)
    {
        var fund = await _fundRepo.GetByIdAsync(dto.FundId)
            ?? throw new ArgumentException("Fund not found.");

        // Use domain methods that enforce business rules (balance checks)
        if (dto.Type == FundTransactionType.Add)
            fund.Deposit(dto.Value);
        else
            fund.Withdraw(dto.Value);

        await _fundRepo.UpdateAsync(fund);

        var transaction = new FundTransaction(dto.Type, dto.Value, dto.FundId, dto.Date);
        await _transactionRepo.AddAsync(transaction);

        return MapToTransactionDto(transaction);
    }

    private static FundTransactionDto MapToTransactionDto(FundTransaction entity) =>
        new(entity.Id, entity.Type, entity.Value, entity.FundId, entity.Date);
}
