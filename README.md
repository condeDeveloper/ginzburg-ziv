# Erdős, Ginzburg e Ziv

Pegue 2n-1 números inteiros quaisquer. O teorema de 1961 diz que sempre dá
para escolher **n** deles cuja soma é divisível por n.

Com 2n-2 não dá: `n-1` zeros e `n-1` uns não têm nenhum subconjunto de n
elementos somando zero módulo n. Então 2n-1 é o número exato — nem um a mais,
nem um a menos.

Este repositório varre **todos** os multiconjuntos dos dois tamanhos, para n de
2 a 8, e responde a pergunta que o teorema não responde: *quantos* escapam com
2n-2, e quais são eles.

## Como rodar

```bash
dotnet test testes/GinzburgZiv.Testes/GinzburgZiv.Testes.csproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- tabela
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- receita
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- fracao
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- limiar
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- preco
```

## Os dois caminhos

- **O juiz** monta todos os subconjuntos de n elementos e soma cada um. Nada de
  tabela, nada de memória.
- **A busca** usa uma tabela de alcance: `alcanca[k][r]` diz se dá para escolher
  k elementos com soma de resto r. Dois subconjuntos que dão o mesmo par
  `(quantos, resto)` viram a mesma casa — e é essa fusão que o juiz não faz.

Os dois foram conferidos multiconjunto por multiconjunto em **54.524** casos,
com zero divergências.

## A varredura

```
n   com 2n-1    falham   com 2n-2    escapam   n*phi(n)/2
2           4         0          3          1            1
3          21         0         15          3            3
4         120         0         84          4            4
5         715         0        495         10           10
6       4.368         0      3.003          6            6
7      27.132         0     18.564         21           21
8     170.544         0    116.280         16           16
```

## O que isto mediu

**O teorema não falhou em nenhum dos 202.904 multiconjuntos de 2n-1
elementos**, de n = 2 a n = 8. E não é que "quase nunca falha": é zero, varrido
um a um, com a testemunha exibida em cada caso.

**Os que escapam com 2n-2 são exatamente `n·φ(n)/2` — e não `C(n,2)`.** Todos
eles são do mesmo formato: duas classes de resto, com `n-1` cópias de cada uma.
Mas não vale qualquer par de classes: a diferença entre elas precisa ser prima
com n. Se `mdc(a-b, n) = d > 1`, basta pegar `n/d` cópias de uma e o resto da
outra, e a soma zera. Por isso em n = 6 escapam **6** e não 15. Zero dos 61
casos medidos ficou fora dessa receita.

**A fuga morre um elemento por vez, e só chega a zero exatamente em 2n-1.**

```
n = 7:  tamanho    7      8     9    10   11   12   13
        escapam  1.470  1.281  560  252   84   21    0
```

A queda não tem fator constante: de 7 para 8 cai 13%, de 8 para 9 cai 56%. Mas
é estritamente decrescente em todos os n medidos, e o último valor positivo cai
sempre em `2n-2`, que é o teorema sendo exato.

**A tabela de alcance só compensa a partir de n = 6.**

```
n   casas de tabela   subconjuntos de n   razao
2                  8                   3      0,4
5                138                 126      0,9
6                256                 462      1,8
8                693               6.435      9,3
10             1.470              92.378     62,8
```

Abaixo de n = 6 a estrutura de dados custa mais do que o problema inteiro.

## O que as medidas me corrigiram

**Apostei que os multiconjuntos extremos fossem todos os pares de classes,
`C(n,2)`.** Escrevi a aposta antes de varrer. Ela acerta em n = 2, 3, 5 e 7 —
todos primos, onde toda diferença é prima com n — e erra em n = 4, 6 e 8. Em
n = 6 são 6 e não 15. O teste guarda as duas coisas: a fórmula que vale e os
três n em que o palpite erra.

**Achei que a tabela de alcance fosse sempre mais barata que enumerar.** Não é:
em n = 2 e n = 3 ela custa duas vezes e meia o preço da força bruta. Otimização
só vale medida, e a medida aqui disse em que n exatamente a conta vira.

**Achei que escapar fosse raro em todo n.** Com n = 2, **um terço** de todos os
multiconjuntos de tamanho 2 escapa. A fração cai para 1,13 por mil em n = 7 e
0,14 por mil em n = 8, mas a frase "é raro" só começa a valer depois de n = 4.

**A varredura para em n = 8, e a tabela diz isso.** Em n = 9 seriam 1.081.575
multiconjuntos de 17 elementos, cada um com mais de mil casas de tabela. A
linha que não foi medida não entra como se tivesse sido.

## Os arquivos

```
src/GinzburgZiv/Nucleo.cs    o multiconjunto como vetor de contagens
src/GinzburgZiv/Juiz.cs      todo subconjunto de n elementos, somado
src/GinzburgZiv/Busca.cs     a tabela de alcance (quantos, resto)
src/GinzburgZiv/Medidas.cs   as tabelas deste README
ferramentas/Medidor/         o executavel que imprime as tabelas
testes/                      51 testes, com os numeros daqui presos um a um
```

51 testes.
