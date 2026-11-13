using Conde.GinzburgZiv;
using Xunit;

namespace Conde.GinzburgZiv.Testes;

/// <summary>Casos pequenos, com a resposta sabida na mao.</summary>
public class NucleoTestes
{
    [Fact]
    public void Tres_zeros_somam_zero_modulo_tres()
    {
        var conta = new[] { 3, 0, 0 };
        Assert.True(Juiz.TemSomaZero(conta, 3, 3));
        Assert.True(Busca.TemSomaZero(conta, 3));
    }

    [Fact]
    public void Dois_zeros_e_dois_uns_nao_tem_tres_somando_zero_modulo_tres()
    {
        // o exemplo classico: n-1 zeros e n-1 uns, com 2n-2 = 4 elementos
        var conta = new[] { 2, 2, 0 };
        Assert.Equal(4, Nucleo.Tamanho(conta));
        Assert.False(Juiz.TemSomaZero(conta, 3, 3));
        Assert.False(Busca.TemSomaZero(conta, 3));
    }

    [Fact]
    public void Com_mais_um_elemento_o_mesmo_exemplo_passa_a_ter()
    {
        foreach (var conta in new[] { new[] { 3, 2, 0 }, new[] { 2, 3, 0 }, new[] { 2, 2, 1 } })
        {
            Assert.Equal(5, Nucleo.Tamanho(conta));
            Assert.True(Busca.TemSomaZero(conta, 3));
        }
    }

    [Fact]
    public void A_testemunha_e_mesmo_um_subconjunto_com_soma_zero()
    {
        for (int n = 2; n <= 6; n++)
            foreach (var conta in Nucleo.Multiconjuntos(n, 2 * n - 1))
            {
                var t = Juiz.Testemunha(conta, n);
                Assert.NotNull(t);
                int soma = 0, quantos = 0;
                for (int r = 0; r < n; r++)
                {
                    Assert.True(t![r] <= conta[r]);
                    quantos += t[r];
                    soma += t[r] * r;
                }
                Assert.Equal(n, quantos);
                Assert.Equal(0, soma % n);
            }
    }

    [Fact]
    public void A_contagem_de_multiconjuntos_e_o_binomial()
    {
        for (int n = 2; n <= 7; n++)
            for (int tamanho = 1; tamanho <= 2 * n - 1; tamanho++)
                Assert.Equal(Nucleo.Binomial(tamanho + n - 1, n - 1),
                             Nucleo.Multiconjuntos(n, tamanho).Count());
    }

    [Fact]
    public void Todo_multiconjunto_gerado_tem_o_tamanho_pedido()
    {
        foreach (var conta in Nucleo.Multiconjuntos(5, 9))
            Assert.Equal(9, Nucleo.Tamanho(conta));
    }

    [Fact]
    public void O_totiente_bate_nos_casos_sabidos()
    {
        Assert.Equal(1, Nucleo.Totiente(1));
        Assert.Equal(1, Nucleo.Totiente(2));
        Assert.Equal(2, Nucleo.Totiente(4));
        Assert.Equal(2, Nucleo.Totiente(6));
        Assert.Equal(6, Nucleo.Totiente(7));
        Assert.Equal(4, Nucleo.Totiente(8));
    }

    [Fact]
    public void Duas_classes_cheias_e_duas_classes_cheias()
    {
        Assert.True(Nucleo.EhDuasClassesCheias(new[] { 2, 2, 0 }, 3));
        Assert.False(Nucleo.EhDuasClassesCheias(new[] { 2, 1, 1 }, 3));
        Assert.False(Nucleo.EhDuasClassesCheias(new[] { 4, 0, 0 }, 3));
        Assert.Equal((0, 1), Nucleo.AsDuasClasses(new[] { 2, 2, 0 }));
        Assert.Equal((0, 2), Nucleo.AsDuasClasses(new[] { 2, 0, 2 }));
    }
}
