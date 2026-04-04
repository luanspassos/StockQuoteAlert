using StockQuoteAlert.Services;
using StockQuoteAlert.Configuration;

Console.WriteLine("=== STOCK QUOTE ALERT ===\n");
Console.WriteLine("Monitor de cotações B3 com alertas por e-mail\n");

var config = new AppConfig();

if (!config.Carregar(args, out string erro))
{
    Console.WriteLine($"ERRO: {erro}");
    return;
}

Console.WriteLine($"Ativo: {config.Simbolo}");
Console.WriteLine($"Venda acima: R$ {config.PrecoVenda:F2}");
Console.WriteLine($"Compra abaixo: R$ {config.PrecoCompra:F2}");
Console.WriteLine($"Email: {config.EmailDestino}");
Console.WriteLine($"Intervalo: {config.IntervaloSegundos}s\n");

Console.WriteLine("Iniciando monitoramento...\n");

var monitorService = new MonitorService();

await monitorService.Executar(config);