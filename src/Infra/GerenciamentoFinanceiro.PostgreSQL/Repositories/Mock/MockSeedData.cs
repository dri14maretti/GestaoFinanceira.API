using GerenciamentoFinanceiro.Entities;
using GerenciamentoFinanceiro.Entities.Enums;

namespace GerenciamentoFinanceiro.PostgreSQL.Repositories.Mock;

/// <summary>
/// Centralized seed data for all mock repositories.
/// Uses fixed GUIDs so relationships between entities are consistent.
/// </summary>
public static class MockSeedData
{
    // --- Category IDs ---
    public static readonly Guid CatAlimentacao = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001");
    public static readonly Guid CatTransporte = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000002");
    public static readonly Guid CatLazer = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000003");
    public static readonly Guid CatSaude = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000004");
    public static readonly Guid CatEducacao = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000005");
    public static readonly Guid CatMoradia = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000006");

    // --- Credit Card IDs ---
    public static readonly Guid CcNubank = Guid.Parse("b2c3d4e5-0001-0000-0000-000000000001");
    public static readonly Guid CcInter = Guid.Parse("b2c3d4e5-0001-0000-0000-000000000002");
    public static readonly Guid CcItau = Guid.Parse("b2c3d4e5-0001-0000-0000-000000000003");

    // --- Investment Category IDs ---
    public static readonly Guid InvCatCDB = Guid.Parse("c3d4e5f6-0001-0000-0000-000000000001");
    public static readonly Guid InvCatTesouro = Guid.Parse("c3d4e5f6-0001-0000-0000-000000000002");
    public static readonly Guid InvCatAcoes = Guid.Parse("c3d4e5f6-0001-0000-0000-000000000003");
    public static readonly Guid InvCatFII = Guid.Parse("c3d4e5f6-0001-0000-0000-000000000004");

    // --- Investment IDs ---
    public static readonly Guid InvCDB1 = Guid.Parse("d4e5f6a7-0001-0000-0000-000000000001");
    public static readonly Guid InvTesouro1 = Guid.Parse("d4e5f6a7-0001-0000-0000-000000000002");
    public static readonly Guid InvAcoes1 = Guid.Parse("d4e5f6a7-0001-0000-0000-000000000003");

    // --- Fund IDs ---
    public static readonly Guid FundEmergencia = Guid.Parse("e5f6a7b8-0001-0000-0000-000000000001");
    public static readonly Guid FundViagem = Guid.Parse("e5f6a7b8-0001-0000-0000-000000000002");
    public static readonly Guid FundCarro = Guid.Parse("e5f6a7b8-0001-0000-0000-000000000003");

    // --- Planning Category IDs ---
    public static readonly Guid PlanCatMoradia = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000001");
    public static readonly Guid PlanCatAlimentacao = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000002");
    public static readonly Guid PlanCatTransporte = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000003");
    public static readonly Guid PlanCatLazer = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000004");
    public static readonly Guid PlanCatInvestimentos = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000005");
    public static readonly Guid PlanCatEmergencia = Guid.Parse("f6a7b8c9-0001-0000-0000-000000000006");

    // --- Monthly Plan IDs ---
    public static readonly Guid PlanMar2026 = Guid.Parse("a7b8c9d0-0001-0000-0000-000000000001");
    public static readonly Guid PlanAbr2026 = Guid.Parse("a7b8c9d0-0001-0000-0000-000000000002");
    public static readonly Guid PlanMai2026 = Guid.Parse("a7b8c9d0-0001-0000-0000-000000000003");
}
