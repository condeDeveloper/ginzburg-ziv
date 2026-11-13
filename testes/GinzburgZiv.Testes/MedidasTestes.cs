using Conde.GinzburgZiv;
using Xunit;

namespace Conde.GinzburgZiv.Testes;

/// <summary>Todo numero do README esta preso aqui.</summary>
public class MedidasTestes
{
    // n, multiconjuntos de 2n-1, multiconjuntos de 2n-2, quantos escapam
    public static TheoryData<int, long, long, long> ATabela() => new()
    {
        { 2, 4, 3, 1 },
        { 3, 21, 15, 3 },
        { 4, 120, 84, 4 },
        { 5, 715, 495, 10 },
        { 6, 4_368, 3_003, 6 },
        { 7, 27_132, 18_564, 21 },
        { 8, 170_544, 116_280, 16 },
    };

    [Theory]
    [MemberData(nameof(ATabela))]
    public void A_tabela_principal(int n, long cheio, long curto, long escapam)
    {
        var l = Medidas.Tabela(8).Single(x => x.N == n);
        Assert.Equal(cheio, l.CheioTotal);
        Assert.Equal(curto, l.CurtoTotal);
        Assert.Equal(escapam, l.CurtoSemSoma);
    }

    [Fact]
    public void Com_2n_menos_1_elementos_nenhum_multiconjunto_escapa()
    {
        foreach (var l in Medidas.Tabela(8))
            Assert.Equal(0, l.CheioSemSoma);
        Assert.Equal(202_904L, Medidas.TotalVarrido(8));
    }

    [Fact]
    public void Os_que_escapam_sao_exatamente_n_vezes_totiente_de_n_sobre_dois()
    {
        foreach (var l in Medidas.Tabela(8))
            Assert.Equal(l.PelaFormula, l.CurtoSemSoma);
    }

    [Fact]
    public void Nenhum_dos_que_escapam_esta_fora_da_receita_de_duas_classes()
    {
        foreach (var (_, _, _, fora) in Medidas.Receita(8))
            Assert.Equal(0, fora);
    }

    [Fact]
    public void A_aposta_de_que_seriam_todos_os_pares_erra_em_tres_dos_sete()
    {
        // C(n,2) conta TODO par de classes; a medida diz que so valem os
        // pares com diferenca prima com n. Em n primo os dois coincidem.
        int erram = 0;
        foreach (var l in Medidas.Tabela(8))
            if (Nucleo.Binomial(l.N, 2) != l.CurtoSemSoma) erram++;
        Assert.Equal(3, erram);
        Assert.Equal(15L, Nucleo.Binomial(6, 2));
        Assert.Equal(6L, Medidas.Tabela(8).Single(x => x.N == 6).CurtoSemSoma);
    }

    [Fact]
    public void O_juiz_e_a_tabela_de_alcance_nao_divergem_em_54524_multiconjuntos()
    {
        Assert.Equal((54_524L, 0L), Medidas.JuizContraBusca(7));
    }

    // n, tamanho, quantos escapam
    public static TheoryData<int, int, long> OLimiar() => new()
    {
        { 5, 5, 100 }, { 5, 6, 80 }, { 5, 7, 40 }, { 5, 8, 10 }, { 5, 9, 0 },
        { 6, 6, 382 }, { 6, 7, 312 }, { 6, 8, 126 }, { 6, 9, 24 }, { 6, 10, 6 }, { 6, 11, 0 },
        { 7, 7, 1_470 }, { 7, 8, 1_281 }, { 7, 9, 560 }, { 7, 10, 252 },
        { 7, 11, 84 }, { 7, 12, 21 }, { 7, 13, 0 },
    };

    [Theory]
    [MemberData(nameof(OLimiar))]
    public void Como_a_fuga_morre_tamanho_por_tamanho(int n, int tamanho, long escapam)
    {
        var linha = Medidas.Limiar(7).Single(x => x.N == n && x.Tamanho == tamanho);
        Assert.Equal(escapam, linha.Escapam);
    }

    [Fact]
    public void A_fuga_cai_a_cada_elemento_a_mais_e_so_zera_em_2n_menos_1()
    {
        for (int n = 2; n <= 7; n++)
        {
            var linhas = Medidas.Limiar(7).Where(x => x.N == n)
                                .OrderBy(x => x.Tamanho).ToArray();
            for (int i = 1; i < linhas.Length; i++)
                Assert.True(linhas[i].Escapam < linhas[i - 1].Escapam);
            Assert.Equal(0, linhas[^1].Escapam);
            Assert.Equal(2 * n - 1, linhas[^1].Tamanho);
            Assert.True(linhas[^2].Escapam > 0);
        }
    }

    // n, casas de tabela, subconjuntos de n elementos
    public static TheoryData<int, long, long> OPreco() => new()
    {
        { 2, 8, 3 },
        { 3, 25, 10 },
        { 4, 63, 35 },
        { 5, 138, 126 },
        { 6, 256, 462 },
        { 7, 438, 1_716 },
        { 8, 693, 6_435 },
        { 9, 1_033, 24_310 },
        { 10, 1_470, 92_378 },
    };

    [Theory]
    [MemberData(nameof(OPreco))]
    public void O_preco_dos_dois_caminhos(int n, long casas, long subconjuntos)
    {
        var (_, c, s, _) = Medidas.Preco(10).Single(x => x.N == n);
        Assert.Equal(casas, c);
        Assert.Equal(subconjuntos, s);
    }

    [Fact]
    public void A_tabela_so_comeca_a_compensar_em_n_igual_a_seis()
    {
        foreach (var (n, _, _, razao) in Medidas.Preco(10))
            if (n <= 5) Assert.True(razao < 1.0);
            else Assert.True(razao > 1.0);
    }

    [Fact]
    public void A_fracao_que_escapa_despenca_de_um_terco_para_um_em_sete_mil()
    {
        var f = Medidas.Fracao(8);
        Assert.Equal(1000.0 / 3, f.Single(x => x.N == 2).PorMil, 6);
        var oito = f.Single(x => x.N == 8);
        Assert.True(oito.PorMil < 0.15);
        Assert.True(oito.PorMil > 0.13);
    }

    [Fact]
    public void O_crivo_de_primo_acerta_os_pequenos()
    {
        var primos = Enumerable.Range(0, 20).Where(Medidas.EhPrimo).ToArray();
        Assert.Equal(new[] { 2, 3, 5, 7, 11, 13, 17, 19 }, primos);
    }
}
