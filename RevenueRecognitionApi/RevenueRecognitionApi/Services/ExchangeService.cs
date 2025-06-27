using System.Text.Json.Serialization;
using RevenueRecognitionApi.Exceptions;

namespace RevenueRecognitionApi.Services;

public interface IExchangeService
{
    public Task<decimal> GetExchangeRateAsync(string from, string to);
}


public class ExchangeService(IHttpClientFactory httpClientFactory) : IExchangeService
{
    public async Task<decimal> GetExchangeRateAsync(string from, string to)
    {
        var url = $"https://open.er-api.com/v6/latest/{from.ToUpper()}";
        
        using var httpClient = httpClientFactory.CreateClient();
        
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Exchange rate API error: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        
        var result = System.Text.Json.JsonSerializer.Deserialize<ExchangeRateApiResult>(json);

        if (result?.Rates == null)
        {
            throw new Exception("Failed to parse exchange rate API response.");
        }

        if (!result.Rates.TryGetValue(to.ToUpper(), out var rate))
        {
            throw new NotFoundException($"Exchange rate from {from} to {to} not found.");
        }

        return rate;
    }

    private class ExchangeRateApiResult
    {
        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("base_code")]
        public string? BaseCode { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}