using StockQuoteAlert.Services;
using StockQuoteAlert.Configuration;

Console.WriteLine("=== STOCK QUOTE ALERT ===\n");
Console.WriteLine("Monitor de cotações B3 com alertas por e-mail\n");

var config = new AppConfig();

if (!config.Load(args, out string error))
{
    Console.WriteLine($"ERRO: {error}");
    return;
}

Console.WriteLine($"Ativo: {config.Symbol}");
Console.WriteLine($"Venda acima: R$ {config.SellPrice:F2}");
Console.WriteLine($"Compra abaixo: R$ {config.BuyPrice:F2}");
Console.WriteLine($"Email: {config.DestinationEmail}");
Console.WriteLine($"Intervalo: {config.IntervalSeconds}s\n");

Console.WriteLine("Iniciando monitoramento...\n");

var monitorService = new MonitorService();

await monitorService.Run(config);