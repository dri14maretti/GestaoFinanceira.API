namespace GerenciamentoFinanceiro.Security;

public class Argon2Options
{
    public const string SectionName = "Argon2";

    public int MemorySizeKiB { get; set; } = 19456; // 19 MiB (OWASP baseline)
    public int Iterations { get; set; } = 2;
    public int DegreeOfParallelism { get; set; } = 1;
    public int SaltByteSize { get; set; } = 16;
    public int HashByteSize { get; set; } = 32;
}
