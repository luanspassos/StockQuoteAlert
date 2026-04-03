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
                string url = $"https://brapi.dev/api/quote/{symbol.ToUpper()}";

                var response = await _http.GetStringAsync(url);
                var json = JsonDocument.Parse(response);
                var root = json.RootElement;

                if (root.TryGetProperty("error", out var error))
                {
                    Console.WriteLine($"Erro da API: {error.GetString()}");
                    return null;
                }

                var results = root.GetProperty("results");

                if (results.GetArrayLength() == 0)
                {
                    Console.WriteLine($"Símbolo {symbol} não encontrado");
                    return null;
                }

                var firstStock = results[0];

                if (!firstStock.TryGetProperty("regularMarketPrice", out var priceElement))
                {
                    Console.WriteLine($"Preço não encontrado para {symbol}");
                    return null;
                }

                double price = priceElement.GetDouble();
                return price;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro de rede: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro ao processar JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return null;
            }
        }
    }
}