Console.WriteLine("=== STOCK QUOTE ALERT ===");
Console.WriteLine("Monitor de cotações B3 com alertas por e-mail\n");

if (args.Length < 3)
{
    Console.WriteLine("Ex: StockQuoteAlert.exe PETR4 22.67 22.59");
    return;
}

string simbolo = args[0];
double precoVenda = double.Parse(args[1]);
double precoCompra = double.Parse(args[2]);


if (string.IsNullOrWhiteSpace(simbolo))
{
    Console.WriteLine("ERRO: Símbolo do ativo não pode estar vazio!");
    return;
}

if (precoVenda <= 0)
{
    Console.WriteLine("ERRO: Preço de venda deve ser maior que zero!");
    return;
}

if (precoCompra <= 0)
{
    Console.WriteLine("ERRO: Preço de compra deve ser maior que zero!");
    return;
}

if (precoVenda <= precoCompra)
{
    Console.WriteLine($"ERRO: Preço de venda ({precoVenda:F2}) deve ser MAIOR que preço de compra ({precoCompra:F2})");
    return;
}

Console.WriteLine("CONFIGURAÇÕES:");
Console.WriteLine($"   Ativo monitorado: {simbolo.ToUpper()}");
Console.WriteLine($"   Preço de VENDA (acima de): R$ {precoVenda:F2}");
Console.WriteLine($"   Preço de COMPRA (abaixo de): R$ {precoCompra:F2}");
Console.WriteLine();

Console.WriteLine("Programa iniciado com sucesso!");