using Microsoft.Extensions.Configuration;
using StockQuoteAlert.Services;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string emailDestino = config["Email:Destino"];
string smtpHost = config["Email:SmtpHost"];
int smtpPort = int.Parse(config["Email:SmtpPort"]);
string smtpUsuario = config["Email:Usuario"];
string smtpSenha = config["Email:Senha"];
bool smtpSsl = bool.Parse(config["Email:EnableSsl"]);
int intervalo = int.Parse(config["Monitor:IntervaloSegundos"]);

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

Console.WriteLine("CONFIGURACOES:");
Console.WriteLine($"Ativo: {simbolo.ToUpper()}");
Console.WriteLine($"Venda acima de: R$ {precoVenda:F2}");
Console.WriteLine($"Compra abaixo de: R$ {precoCompra:F2}");
Console.WriteLine($"Email alerta: {emailDestino}");
Console.WriteLine($"Intervalo: {intervalo} segundos");
Console.WriteLine();

Console.WriteLine("Iniciando monitoramento...\n");

await Executar();

async Task Executar()
{
    var quoteService = new QuoteService();

    Console.WriteLine($"Buscando cotacao de {simbolo}...");
    double? preco = await quoteService.GetPrice(simbolo);

    if (preco.HasValue)
    {
        Console.WriteLine($"Preco atual: R$ {preco.Value:F2}");

        if (preco.Value > precoVenda)
            Console.WriteLine(">>> ACIMA do preco de VENDA");
        else if (preco.Value < precoCompra)
            Console.WriteLine(">>> ABAIXO do preco de COMPRA");
        else
            Console.WriteLine(">>> Dentro da faixa normal");
    }
    else
    {
        Console.WriteLine("Erro ao obter cotacao");
    }

    Console.WriteLine("\nPressione qualquer tecla para sair...");
    Console.ReadKey();
}