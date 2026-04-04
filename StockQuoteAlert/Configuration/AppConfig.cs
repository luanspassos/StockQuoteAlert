using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace StockQuoteAlert.Configuration;

public class AppConfig
{
    public string DestinationEmail { get; private set; } = "";
    public string SmtpHost { get; private set; } = "";
    public int SmtpPort { get; private set; }
    public string SmtpUser { get; private set; } = "";
    public string SmtpPassword { get; private set; } = "";
    public bool SmtpSsl { get; private set; }
    public int IntervalSeconds { get; private set; }
    public string Symbol { get; private set; } = "";
    public double SellPrice { get; private set; }
    public double BuyPrice { get; private set; }

    public bool Load(string[] args, out string error)
    {
        error = "";

        IConfigurationRoot config;
        try
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }
        catch (FileNotFoundException)
        {
            error = "Arquivo appsettings.json não encontrado. Verifique se ele existe na pasta do projeto.";
            return false;
        }
        catch (Exception ex)
        {
            error = $"Erro ao ler appsettings.json: {ex.Message}";
            return false;
        }

        DestinationEmail = config["Email:Destino"] ?? "";
        if (string.IsNullOrWhiteSpace(DestinationEmail))
        {
            error = "Email:Destino não configurado no appsettings.json";
            return false;
        }

        SmtpHost = config["Email:SmtpHost"] ?? "";
        if (string.IsNullOrWhiteSpace(SmtpHost))
        {
            error = "Email:SmtpHost não configurado no appsettings.json";
            return false;
        }

        string? smtpPortStr = config["Email:SmtpPort"];
        if (!int.TryParse(smtpPortStr, out int smtpPort))
        {
            error = $"Email:SmtpPort inválido: '{smtpPortStr}'. Deve ser um número (ex: 587)";
            return false;
        }
        SmtpPort = smtpPort;

        SmtpUser = config["Email:Usuario"] ?? "";
        if (string.IsNullOrWhiteSpace(SmtpUser))
        {
            error = "Email:Usuario não configurado no appsettings.json";
            return false;
        }

        SmtpPassword = config["Email:Senha"] ?? "";
        if (string.IsNullOrWhiteSpace(SmtpPassword))
        {
            error = "Email:Senha não configurado no appsettings.json";
            return false;
        }

        string? smtpSslStr = config["Email:EnableSsl"];
        if (!bool.TryParse(smtpSslStr, out bool smtpSsl))
        {
            error = $"Email:EnableSsl inválido: '{smtpSslStr}'. Deve ser true ou false";
            return false;
        }
        SmtpSsl = smtpSsl;

        string? intervalSecondsStr = config["Monitor:IntervaloSegundos"];
        if (!int.TryParse(intervalSecondsStr, out int intervalSeconds))
        {
            error = $"Monitor:IntervaloSegundos inválido: '{intervalSecondsStr}'. Deve ser um número (ex: 30)";
            return false;
        }
        IntervalSeconds = intervalSeconds;

        if (args.Length < 3)
        {
            error = "Uso: dotnet run -- <SIMBOLO> <PRECO_VENDA> <PRECO_COMPRA>";
            return false;
        }

        Symbol = args[0].Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(Symbol))
        {
            error = "Símbolo do ativo não pode estar vazio";
            return false;
        }

        if (!double.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double sellPrice))
        {
            error = $"Preço de venda inválido: '{args[1]}'. Use ponto como separador decimal (ex: 22.67)";
            return false;
        }
        SellPrice = sellPrice;

        if (!double.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double buyPrice))
        {
            error = $"Preço de compra inválido: '{args[2]}'. Use ponto como separador decimal (ex: 22.59)";
            return false;
        }
        BuyPrice = buyPrice;

        if (SellPrice <= 0)
        {
            error = "Preço de venda deve ser maior que zero";
            return false;
        }

        if (BuyPrice <= 0)
        {
            error = "Preço de compra deve ser maior que zero";
            return false;
        }

        if (SellPrice <= BuyPrice)
        {
            error = $"Preço de venda ({SellPrice:F2}) deve ser MAIOR que preço de compra ({BuyPrice:F2})";
            return false;
        }

        return true;
    }
}