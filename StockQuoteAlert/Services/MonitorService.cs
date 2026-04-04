using StockQuoteAlert.Configuration;

namespace StockQuoteAlert.Services;

public class MonitorService
{
    public async Task Run(AppConfig config)
    {
        var quoteService = new QuoteService();
        var emailService = new EmailService(config.SmtpHost, config.SmtpPort, config.SmtpUser, config.SmtpPassword, config.SmtpSsl);
        string state = "normal";

        while (true)
        {
            try
            {
                Console.WriteLine($"Buscando cotacao de {config.Symbol}...");
                double? price = await quoteService.GetPrice(config.Symbol);

                if (!price.HasValue)
                {
                    Console.WriteLine("Erro ao obter cotacao, tentando novamente...");
                    await Task.Delay(config.IntervalSeconds * 1000);
                    continue;
                }

                Console.WriteLine($"Preco atual: R$ {price.Value:F2}");

                if (price.Value > config.SellPrice && state != "acima")
                {
                    Console.WriteLine(">>> ACIMA do preco de VENDA - Enviando email...");

                    string subject = $"[ALERTA] {config.Symbol} - VENDER";
                    string body = $"{config.Symbol} atingiu R$ {price.Value:F2}\n" +
                                   $"Acima do preco de venda: R$ {config.SellPrice:F2}\n" +
                                   $"Sugestao: VENDER\n" +
                                   $"Hora: {DateTime.Now:HH:mm:ss}";

                    emailService.Send(config.DestinationEmail, subject, body);
                    Console.WriteLine(">>> Email de VENDA enviado com sucesso!");
                    state = "acima";
                }
                else if (price.Value < config.BuyPrice && state != "abaixo")
                {
                    Console.WriteLine(">>> ABAIXO do preco de COMPRA - Enviando email...");
                    string assunto = $"[ALERTA] {config.Symbol} - COMPRAR";
                    string corpo = $"{config.Symbol} atingiu R$ {price.Value:F2}\n" +
                                   $"Abaixo do preco de compra: R$ {config.BuyPrice:F2}\n" +
                                   $"Sugestao: COMPRAR\n" +
                                   $"Hora: {DateTime.Now:HH:mm:ss}";

                    emailService.Send(config.DestinationEmail, assunto, corpo);
                    Console.WriteLine(">>> Email de COMPRA enviado com sucesso!");
                    state = "abaixo";
                }
                else if (price.Value <= config.SellPrice && price.Value >= config.BuyPrice)
                {
                    if (state != "normal")
                    {
                        Console.WriteLine(">>> Preco voltou a faixa normal");
                        state = "normal";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

            Console.WriteLine($"Aguardando {config.IntervalSeconds} segundos para proxima verificacao...\n");
            await Task.Delay(config.IntervalSeconds * 1000);
        }
    }
}