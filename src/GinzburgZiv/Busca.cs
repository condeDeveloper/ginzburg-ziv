namespace Conde.GinzburgZiv;

/// <summary>
/// A rota rapida: uma tabela de alcance. <c>alcanca[k][r]</c> diz se da para
/// escolher k elementos com soma de resto r, e ela cresce classe por classe.
///
/// Nao enumera subconjunto nenhum. Dois subconjuntos diferentes que dao o
/// mesmo par (quantos, resto) viram a mesma casa da tabela, e e exatamente
/// essa fusao que o <see cref="Juiz"/> nao faz.
/// </summary>
public static class Busca
{
    /// <summary>Quantas casas de tabela a ultima chamada mexeu.</summary>
    public static long UltimoCusto { get; private set; }

    /// <summary>
    /// A tabela inteira: saida[k][r] e verdadeiro quando existe subconjunto
    /// de k elementos com soma de resto r.
    /// </summary>
    public static bool[][] Alcance(IReadOnlyList<int> conta, int n, int ate)
    {
        var alcanca = new bool[ate + 1][];
        for (int k = 0; k <= ate; k++) alcanca[k] = new bool[n];
        alcanca[0][0] = true;
        long custo = 0;

        for (int r = 0; r < n; r++)
        {
            int c = conta[r];
            if (c == 0) continue;
            var novo = new bool[ate + 1][];
            for (int k = 0; k <= ate; k++) novo[k] = new bool[n];

            for (int k = 0; k <= ate; k++)
                for (int res = 0; res < n; res++)
                {
                    if (!alcanca[k][res]) continue;
                    int soma = res;
                    for (int t = 0; k + t <= ate && t <= c; t++)
                    {
                        novo[k + t][soma] = true;
                        soma = (soma + r) % n;
                        custo++;
                    }
                }
            alcanca = novo;
        }

        UltimoCusto = custo;
        return alcanca;
    }

    /// <summary>Existe subconjunto de n elementos com soma divisivel por n?</summary>
    public static bool TemSomaZero(IReadOnlyList<int> conta, int n)
        => Alcance(conta, n, n)[n][0];

    /// <summary>
    /// Varre TODOS os multiconjuntos de um tamanho e conta quantos nao tem
    /// subconjunto de n elementos somando zero.
    /// </summary>
    public static (long Total, long SemSoma, List<int[]> Escapam) Varre(int n, int tamanho)
    {
        long total = 0, sem = 0;
        var escapam = new List<int[]>();
        foreach (var conta in Nucleo.Multiconjuntos(n, tamanho))
        {
            total++;
            if (!TemSomaZero(conta, n))
            {
                sem++;
                escapam.Add((int[])conta.Clone());
            }
        }
        return (total, sem, escapam);
    }
}
