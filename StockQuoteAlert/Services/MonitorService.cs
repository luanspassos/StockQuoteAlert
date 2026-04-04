using StockQuoteAlert.Configuration;

namespace StockQuoteAlert.Services;

public class MonitorService
{
    public async Task Executar(AppConfig config)
    {
        var quoteService = new QuoteService();
        var emailService = new EmailService(config.SmtpHost, config.SmtpPort, config.SmtpUsuario, config.SmtpSenha, config.SmtpSsl);
        string estado = "normal";

        while (true)
        {
            try
            {
                Console.WriteLine($"Buscando cotacao de {config.Simbolo}...");
                double? preco = await quoteService.GetPrice(config.Simbolo);

                if (!preco.HasValue)
                {
                    Console.WriteLine("Erro ao obter cotacao, tentando novamente...");
                    await Task.Delay(config.IntervaloSegundos * 1000);
                    continue;
                }

                Console.WriteLine($"Preco atual: R$ {preco.Value:F2}");

                if (preco.Value > config.PrecoVenda && estado != "acima")
                {
                    estado = "acima";
                    Console.WriteLine(">>> ACIMA do preco de VENDA - Enviando email...");

                    string assunto = $"[ALERTA] {config.Simbolo} - VENDER";
                    string corpo = $"{config.Simbolo} atingiu R$ {preco.Value:F2}\n" +
                                   $"Acima do preco de venda: R$ {config.PrecoVenda:F2}\n" +
                                   $"Sugestao: VENDER\n" +
                                   $"Hora: {DateTime.Now:HH:mm:ss}";

                    emailService.Send(config.EmailDestino, assunto, corpo);
                    Console.WriteLine(">>> Email de VENDA enviado com sucesso!");
                }
                else if (preco.Value < config.PrecoCompra && estado != "abaixo")
                {
                    estado = "abaixo";
                    Console.WriteLine(">>> ABAIXO do preco de COMPRA - Enviando email...");
                    string assunto = $"[ALERTA] {config.Simbolo} - COMPRAR";
                    string corpo = $"{config.Simbolo} atingiu R$ {preco.Value:F2}\n" +
                                   $"Abaixo do preco de compra: R$ {config.PrecoCompra:F2}\n" +
                                   $"Sugestao: COMPRAR\n" +
                                   $"Hora: {DateTime.Now:HH:mm:ss}";

                    emailService.Send(config.EmailDestino, assunto, corpo);
                    Console.WriteLine(">>> Email de COMPRA enviado com sucesso!");
                }
                else if (preco.Value <= config.PrecoVenda && preco.Value >= config.PrecoCompra)
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
                Console.WriteLine($"Erro: {ex.Message}");
            }

            Console.WriteLine($"Aguardando {config.IntervaloSegundos} segundos para proxima verificacao...\n");
            await Task.Delay(config.IntervaloSegundos * 1000);
        }
    }
}