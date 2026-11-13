namespace Conde.GinzburgZiv;

/// <summary>
/// O teorema de Erdos, Ginzburg e Ziv, de 1961: de quaisquer 2n-1 inteiros,
/// sempre da para escolher n cuja soma e divisivel por n.
///
/// Com 2n-2 nao da: n-1 zeros e n-1 uns nao tem subconjunto nenhum de n
/// elementos somando zero modulo n. Entao 2n-1 e o numero exato, nem um a
/// mais nem um a menos — e e isso que este repositorio varre inteiro.
///
/// Um multiconjunto vira um vetor de contagens: <c>conta[r]</c> e quantos
/// elementos deixam resto r na divisao por n.
/// </summary>
public static class Nucleo
{
    /// <summary>Quantos elementos o multiconjunto tem.</summary>
    public static int Tamanho(IReadOnlyList<int> conta)
    {
        int s = 0;
        foreach (int c in conta) s += c;
        return s;
    }

    /// <summary>
    /// Todos os multiconjuntos de tamanho <paramref name="tamanho"/> com
    /// restos de 0 a n-1, um de cada vez. Sao C(tamanho + n - 1, n - 1).
    /// </summary>
    public static IEnumerable<int[]> Multiconjuntos(int n, int tamanho)
    {
        var conta = new int[n];

        IEnumerable<int[]> Anda(int resto, int sobra)
        {
            if (resto == n - 1)
            {
                conta[resto] = sobra;
                // vetor de TRABALHO: quem guardar tem de copiar
                yield return conta;
                yield break;
            }
            for (int q = 0; q <= sobra; q++)
            {
                conta[resto] = q;
                foreach (var r in Anda(resto + 1, sobra - q)) yield return r;
            }
        }

        return Anda(0, tamanho);
    }

    /// <summary>C(n, k), sem fatorial.</summary>
    public static long Binomial(int n, int k)
    {
        if (k < 0 || k > n) return 0;
        k = Math.Min(k, n - k);
        long r = 1;
        for (int i = 1; i <= k; i++)
            r = r * (n - k + i) / i;
        return r;
    }

    /// <summary>A funcao de Euler: quantos de 1 a n sao primos com n.</summary>
    public static int Totiente(int n)
    {
        int q = 0;
        for (int k = 1; k <= n; k++)
            if (Mdc(k, n) == 1) q++;
        return q;
    }

    /// <summary>O maximo divisor comum.</summary>
    public static int Mdc(int a, int b)
    {
        while (b != 0) (a, b) = (b, a % b);
        return a;
    }

    /// <summary>
    /// O multiconjunto e do formato extremo: duas classes so, com n-1 copias
    /// de cada uma.
    /// </summary>
    public static bool EhDuasClassesCheias(IReadOnlyList<int> conta, int n)
    {
        int classes = 0;
        for (int r = 0; r < n; r++)
        {
            if (conta[r] == 0) continue;
            if (conta[r] != n - 1) return false;
            classes++;
        }
        return classes == 2;
    }

    /// <summary>As duas classes de um multiconjunto que tem exatamente duas.</summary>
    public static (int A, int B) AsDuasClasses(IReadOnlyList<int> conta)
    {
        int a = -1, b = -1;
        for (int r = 0; r < conta.Count; r++)
        {
            if (conta[r] == 0) continue;
            if (a < 0) a = r;
            else b = r;
        }
        return (a, b);
    }
}
