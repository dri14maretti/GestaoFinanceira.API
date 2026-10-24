using System;

namespace GerenciamentoFinanceiro.Entities;

public interface IUserOwnedEntity
{
    Guid UserId { get; }
    void AssignOwner(Guid userId);
}
