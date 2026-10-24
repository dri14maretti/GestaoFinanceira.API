using System;

namespace GerenciamentoFinanceiro.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid GetRequiredUserId();
}
