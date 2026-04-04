using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace StockQuoteAlert.Configuration;

public class AppConfig
{
    public string EmailDestino { get; private set; } = "";
    public string SmtpHost { get; private set; } = "";
    public int SmtpPort { get; private set; }
    public string SmtpUsuario { get; private set; } = "";
    public string SmtpSenha { get; private set; } = "";
    public bool SmtpSsl { get; private set; }
    public int IntervaloSegundos { get; private set; }
    public string Simbolo { get; private set; } = "";
    public double PrecoVenda { get; private set; }
    public double PrecoCompra { get; private set; }

    public bool Carregar(string[] args, out string erro)
    {
        erro = "";

        var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

        EmailDestino = config["Email:Destino"] ?? "";
        SmtpHost = config["Email:SmtpHost"] ?? "";
        SmtpPort = int.Parse(config["Email:SmtpPort"] ?? "587");
        SmtpUsuario = config["Email:Usuario"] ?? "";
        SmtpSenha = config["Email:Senha"] ?? "";
        SmtpSsl = bool.Parse(config["Email:EnableSsl"] ?? "true");
        IntervaloSegundos = int.Parse(config["Monitor:IntervaloSegundos"] ?? "30");

        if (args.Length < 3)
        {
            erro = "Ex: StockQuoteAlert.exe PETR4 22.67 22.59";
            return false;
        }

        Simbolo = args[0];
        PrecoVenda = double.Parse(args[1], CultureInfo.InvariantCulture);
        PrecoCompra = double.Parse(args[2], CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(Simbolo))
        {
            erro = "ERRO: Símbolo do ativo não pode estar vazio!";
            return false;
        }

        if (PrecoVenda <= 0)
        {
            erro = "ERRO: Preço de venda deve ser maior que zero!";
            return false;
        }

        if (PrecoCompra <= 0)
        {
            erro = "ERRO: Preço de compra deve ser maior que zero!";
            return false;
        }

        if (PrecoVenda <= PrecoCompra)
        {
            erro = $"ERRO: Preço de venda ({PrecoVenda:F2}) deve ser MAIOR que preço de compra ({PrecoCompra:F2})";
            return false;
        }

        return true;
    }
}