namespace Conde.GinzburgZiv;

/// <summary>As tabelas do README. Todo numero de la sai daqui.</summary>
public static class Medidas
{
    private static readonly Dictionary<string, object> Cache = [];

    private static T Guardado<T>(string chave, Func<T> faz)
    {
        lock (Cache)
        {
            if (Cache.TryGetValue(chave, out var pronto)) return (T)pronto;
            var feito = faz()!;
            Cache[chave] = feito;
            return feito;
        }
    }

    /// <summary>
    /// A tabela principal: para cada n, os multiconjuntos de 2n-1 elementos
    /// (onde o teorema diz que nunca falha) e os de 2n-2 (onde ele pode).
    /// </summary>
    public static IReadOnlyList<Linha> Tabela(int ate = 8)
        => Guardado("Tabela" + ate, () => CalculaTabela(ate));

    private static IReadOnlyList<Linha> CalculaTabela(int ate)
    {
        var linhas = new List<Linha>();
        for (int n = 2; n <= ate; n++)
        {
            var cheio = Busca.Varre(n, 2 * n - 1);
            var curto = Busca.Varre(n, 2 * n - 2);
            linhas.Add(new Linha(n, cheio.Total, cheio.SemSoma, curto.Total,
                                 curto.SemSoma, (long)n * Nucleo.Totiente(n) / 2,
                                 curto.Escapam));
        }
        return linhas;
    }

    /// <summary>
    /// O juiz contra a tabela de alcance, multiconjunto por multiconjunto,
    /// nos dois tamanhos e em todo n ate <paramref name="ate"/>.
    /// </summary>
    public static (long Conferidos, long Divergem) JuizContraBusca(int ate = 7)
        => Guardado("Juiz" + ate, () => CalculaJuizContraBusca(ate));

    private static (long Conferidos, long Divergem) CalculaJuizContraBusca(int ate)
    {
        long conferidos = 0, divergem = 0;
        for (int n = 2; n <= ate; n++)
            for (int tamanho = 2 * n - 2; tamanho <= 2 * n - 1; tamanho++)
                foreach (var conta in Nucleo.Multiconjuntos(n, tamanho))
                {
                    conferidos++;
                    if (Juiz.TemSomaZero(conta, n, n) != Busca.TemSomaZero(conta, n))
                        divergem++;
                }
        return (conferidos, divergem);
    }

    /// <summary>
    /// Os multiconjuntos de 2n-2 que escapam, conferidos contra a receita:
    /// duas classes com n-1 copias cada, e a diferenca entre elas prima com n.
    /// </summary>
    public static IReadOnlyList<(int N, long Escapam, long PelaReceita, int ForaDaReceita)> Receita(int ate = 8)
        => Guardado("Receita" + ate, () => CalculaReceita(ate));

    private static IReadOnlyList<(int N, long Escapam, long PelaReceita, int ForaDaReceita)> CalculaReceita(int ate)
    {
        var saida = new List<(int, long, long, int)>();
        foreach (var l in Tabela(ate))
        {
            int fora = 0;
            foreach (var conta in l.Escapam)
            {
                if (!Nucleo.EhDuasClassesCheias(conta, l.N)) { fora++; continue; }
                var (a, b) = Nucleo.AsDuasClasses(conta);
                if (Nucleo.Mdc(Math.Abs(a - b), l.N) != 1) fora++;
            }
            saida.Add((l.N, l.CurtoSemSoma, l.PelaFormula, fora));
        }
        return saida;
    }

    /// <summary>
    /// O custo dos dois caminhos num multiconjunto so: quantas casas de
    /// tabela a busca mexe contra quantos subconjuntos de n elementos existem.
    /// </summary>
    public static IReadOnlyList<(int N, long CasasDeTabela, long Subconjuntos, double Razao)> Preco(int ate = 10)
        => Guardado("Preco" + ate, () => CalculaPreco(ate));

    private static IReadOnlyList<(int N, long CasasDeTabela, long Subconjuntos, double Razao)> CalculaPreco(int ate)
    {
        var saida = new List<(int, long, long, double)>();
        for (int n = 2; n <= ate; n++)
        {
            // o pior caso para os dois: todos os restos presentes
            var conta = new int[n];
            for (int i = 0; i < 2 * n - 1; i++) conta[i % n]++;
            Busca.Alcance(conta, n, n);
            long casas = Busca.UltimoCusto;
            long subconjuntos = Nucleo.Binomial(2 * n - 1, n);
            saida.Add((n, casas, subconjuntos, (double)subconjuntos / casas));
        }
        return saida;
    }

    /// <summary>
    /// A pergunta que sobra: com 2n-2 elementos, que fracao dos
    /// multiconjuntos escapa? Ela nao e uma curva lisa.
    /// </summary>
    public static IReadOnlyList<(int N, long Total, long Escapam, double PorMil, bool Primo)> Fracao(int ate = 8)
    {
        var saida = new List<(int, long, long, double, bool)>();
        foreach (var l in Tabela(ate))
            saida.Add((l.N, l.CurtoTotal, l.CurtoSemSoma,
                       1000.0 * l.CurtoSemSoma / l.CurtoTotal, EhPrimo(l.N)));
        return saida;
    }

    /// <summary>
    /// Tamanho por tamanho, de n ate 2n-1: quantos multiconjuntos escapam.
    /// O teorema so fala do 2n-1; a pergunta de como a fuga MORRE no caminho
    /// ate la ninguem responde, e ela se responde contando.
    /// </summary>
    public static IReadOnlyList<(int N, int Tamanho, long Total, long Escapam)> Limiar(int ate = 7)
        => Guardado("Limiar" + ate, () => CalculaLimiar(ate));

    private static IReadOnlyList<(int N, int Tamanho, long Total, long Escapam)> CalculaLimiar(int ate)
    {
        var saida = new List<(int, int, long, long)>();
        for (int n = 2; n <= ate; n++)
            for (int tamanho = n; tamanho <= 2 * n - 1; tamanho++)
            {
                var (total, sem, _) = Busca.Varre(n, tamanho);
                saida.Add((n, tamanho, total, sem));
            }
        return saida;
    }

    /// <summary>Quantos multiconjuntos de 2n-1 foram varridos ao todo.</summary>
    public static long TotalVarrido(int ate = 8) => Tabela(ate).Sum(l => l.CheioTotal);

    /// <summary>Verdadeiro quando p e primo.</summary>
    public static bool EhPrimo(int p)
    {
        if (p < 2) return false;
        for (int d = 2; (long)d * d <= p; d++)
            if (p % d == 0) return false;
        return true;
    }
}

/// <summary>Uma linha da tabela principal.</summary>
public sealed record Linha(int N, long CheioTotal, long CheioSemSoma,
                           long CurtoTotal, long CurtoSemSoma, long PelaFormula,
                           List<int[]> Escapam);
