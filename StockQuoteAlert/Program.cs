using Microsoft.Extensions.Configuration;
using StockQuoteAlert.Services;
using System.Globalization;

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
double precoVenda = double.Parse(args[1], CultureInfo.InvariantCulture);
double precoCompra = double.Parse(args[2], CultureInfo.InvariantCulture);

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
    var emailService = new EmailService(smtpHost, smtpPort, smtpUsuario, smtpSenha, smtpSsl);

    string estado = "normal";

    while (true)
    {
        try
        {
            Console.WriteLine($"Buscando cotacao de {simbolo}...");
            double? preco = await quoteService.GetPrice(simbolo);

            if (!preco.HasValue)
            {
                Console.WriteLine("Erro ao obter cotacao, tentando novamente...");
                await Task.Delay(intervalo * 1000);
                continue;
            }

            Console.WriteLine($"Preco atual: R$ {preco.Value:F2}");

            if (preco.Value > precoVenda && estado != "acima")
            {
                estado = "acima";
                Console.WriteLine(">>> ACIMA do preco de VENDA - Enviando email...");

                string assunto = $"[ALERTA] {simbolo} - VENDER";
                string corpo = $"{simbolo} atingiu R$ {preco.Value:F2}\n" +
                               $"Acima do preco de venda: R$ {precoVenda:F2}\n" +
                               $"Sugestao: VENDER\n" +
                               $"Hora: {DateTime.Now:HH:mm:ss}";

                emailService.Send(emailDestino, assunto, corpo);
                Console.WriteLine(">>> Email de VENDA enviado com sucesso!");
            }
            else if (preco.Value < precoCompra && estado != "abaixo")
            {
                estado = "abaixo";
                Console.WriteLine(">>> ABAIXO do preco de COMPRA - Enviando email...");

                string assunto = $"[ALERTA] {simbolo} - COMPRAR";
                string corpo = $"{simbolo} atingiu R$ {preco.Value:F2}\n" +
                               $"Abaixo do preco de compra: R$ {precoCompra:F2}\n" +
                               $"Sugestao: COMPRAR\n" +
                               $"Hora: {DateTime.Now:HH:mm:ss}";

                emailService.Send(emailDestino, assunto, corpo);
                Console.WriteLine(">>> Email de COMPRA enviado com sucesso!");
            }
            else if (preco.Value <= precoVenda && preco.Value >= precoCompra)
            {
                if (estado != "normal")
                {
                    Console.WriteLine(">>> Preco voltou a faixa normal");
                    estado = "normal";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no monitoramento: {ex.Message}");
        }

        Console.WriteLine($"Aguardando {intervalo} segundos para proxima verificacao...\n");
        await Task.Delay(intervalo * 1000);
    }
}