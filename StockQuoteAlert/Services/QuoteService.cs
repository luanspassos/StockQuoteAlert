using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockQuoteAlert.Services
{
    public class QuoteService
    {
        private readonly HttpClient _http = new HttpClient();

        public async Task<double?> GetPrice(string symbol)
        {
            try
            {
                string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}.SA&apikey=demo";

                var response = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var root = json.RootElement;

                if (root.TryGetProperty("Error Message", out _))
                {
                    Console.WriteLine("Erro: Limite da API ou simbolo invalido");
                    return null;
                }

                var globalQuote = root.GetProperty("Global Quote");

                if (!globalQuote.TryGetProperty("05. price", out var priceElement))
                {
                    Console.WriteLine($"Simbolo {symbol} nao encontrado");
                    return null;
                }

                string priceStr = priceElement.GetString();
                return double.Parse(priceStr, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na API: {ex.Message}");
                return null;
            }
        }
    }
}