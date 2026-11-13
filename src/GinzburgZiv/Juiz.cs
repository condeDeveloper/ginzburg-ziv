namespace Conde.GinzburgZiv;

/// <summary>
/// O juiz e a definicao: ele monta TODOS os subconjuntos de n elementos e
/// soma cada um. Nao tem tabela, nao tem memoria, nao reaproveita nada.
///
/// Em n = 8 um multiconjunto de 15 elementos tem C(15,8) = 6.435 subconjuntos,
/// e sao 170.544 multiconjuntos para olhar: por isso o juiz fica limitado aos
/// n pequenos e a <see cref="Busca"/> leva as tabelas adiante.
/// </summary>
public static class Juiz
{
    /// <summary>
    /// Existe um subconjunto de exatamente <paramref name="quantos"/>
    /// elementos cuja soma e divisivel por n?
    /// </summary>
    public static bool TemSomaZero(IReadOnlyList<int> conta, int n, int quantos)
    {
        var escolha = new int[n];

        bool Anda(int resto, int faltam, int soma)
        {
            if (resto == n)
                return faltam == 0 && soma % n == 0;
            int maximo = Math.Min(conta[resto], faltam);
            for (int q = 0; q <= maximo; q++)
            {
                escolha[resto] = q;
                if (Anda(resto + 1, faltam - q, (soma + q * resto) % n)) return true;
            }
            return false;
        }

        return Anda(0, quantos, 0);
    }

    /// <summary>
    /// Um subconjunto de n elementos com soma zero, se existir: as contagens
    /// por resto. Serve para mostrar a testemunha, nao so dizer que ela existe.
    /// </summary>
    public static int[]? Testemunha(IReadOnlyList<int> conta, int n)
    {
        var escolha = new int[n];

        bool Anda(int resto, int faltam, int soma)
        {
            if (resto == n)
                return faltam == 0 && soma % n == 0;
            int maximo = Math.Min(conta[resto], faltam);
            for (int q = 0; q <= maximo; q++)
            {
                escolha[resto] = q;
                if (Anda(resto + 1, faltam - q, (soma + q * resto) % n)) return true;
            }
            escolha[resto] = 0;
            return false;
        }

        return Anda(0, n, 0) ? escolha : null;
    }
}
