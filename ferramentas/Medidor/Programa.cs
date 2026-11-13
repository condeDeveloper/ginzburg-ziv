using Conde.GinzburgZiv;

if (args.Length == 0)
{
    Console.WriteLine("medidor <tabela|juiz|receita|fracao|limiar|preco|tudo>");
    return 1;
}

switch (args[0])
{
    case "tabela": Tabela(); break;
    case "juiz": JuizContraBusca(); break;
    case "receita": Receita(); break;
    case "fracao": Fracao(); break;
    case "limiar": Limiar(); break;
    case "preco": Preco(); break;
    case "tudo": Tabela(); JuizContraBusca(); Receita(); Fracao(); Limiar(); Preco(); break;
    default:
        Console.WriteLine($"nao conheco '{args[0]}'");
        return 1;
}
return 0;

void Tabela()
{
    Console.WriteLine("com 2n-1 elementos o teorema diz que nunca falha;");
    Console.WriteLine("com 2n-2 ele pode falhar, e falha.");
    Console.WriteLine();
    Console.WriteLine("n   com 2n-1    falham   com 2n-2    escapam   n*phi(n)/2");
    foreach (var l in Medidas.Tabela(8))
        Console.WriteLine($"{l.N,-3} {l.CheioTotal,9:n0} {l.CheioSemSoma,9:n0} " +
                          $"{l.CurtoTotal,10:n0} {l.CurtoSemSoma,10:n0} {l.PelaFormula,12:n0}");
    Console.WriteLine();
    Console.WriteLine($"ao todo, {Medidas.TotalVarrido(8):n0} multiconjuntos de 2n-1 varridos, " +
                      "e nenhum escapou do teorema.");
}

void JuizContraBusca()
{
    var (conf, div) = Medidas.JuizContraBusca(7);
    Console.WriteLine($"juiz por subconjunto contra a tabela de alcance: " +
                      $"{conf:n0} multiconjuntos, {div} divergem");
}

void Receita()
{
    Console.WriteLine("n   escapam   pela receita   fora da receita");
    foreach (var (n, escapam, formula, fora) in Medidas.Receita(8))
        Console.WriteLine($"{n,-3} {escapam,9:n0} {formula,14:n0} {fora,17}");
}

void Fracao()
{
    Console.WriteLine("n   com 2n-2    escapam   por mil   n primo");
    foreach (var (n, total, escapam, porMil, primo) in Medidas.Fracao(8))
        Console.WriteLine($"{n,-3} {total,10:n0} {escapam,10:n0} {porMil,9:n2}   {(primo ? "sim" : "nao")}");
}

void Preco()
{
    Console.WriteLine("n   casas de tabela   subconjuntos de n   razao");
    foreach (var (n, casas, sub, razao) in Medidas.Preco(10))
        Console.WriteLine($"{n,-3} {casas,16:n0} {sub,19:n0} {razao,8:n1}");
}

void Limiar()
{
    Console.WriteLine("n   tamanho   multiconjuntos   escapam");
    foreach (var (n, tamanho, total, escapam) in Medidas.Limiar(7))
        Console.WriteLine($"{n,-3} {tamanho,9} {total,16:n0} {escapam,9:n0}");
}
