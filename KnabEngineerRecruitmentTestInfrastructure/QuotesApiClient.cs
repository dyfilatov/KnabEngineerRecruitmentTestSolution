using System.Net.Http.Json;
using KnabEngineerRecruitmentTestInfrastructure.Models;
using KnabEngineerRecruitmentTestInfrastructure.Models.CoinMarketCap;
using KnabEngineerRecruitmentTestInfrastructure.Models.Dto;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace KnabEngineerRecruitmentTestInfrastructure;

internal class QuotesApiClient : IQuotesApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ExternalQuotesApiSettings _settings;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public QuotesApiClient(IHttpClientFactory httpClientFactory,
        IOptions<ExternalQuotesApiSettings> settings,
        IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _settings = settings.Value;
    }

    public async Task<List<QuoteDto>> GetLatestQuotesAsync(
        QuotesSource quotesSource, string fromCurrency,
        IEnumerable<string> toCurrencies)
    {
        var cacheKey = $"{quotesSource},{fromCurrency},{string.Join(',', toCurrencies)}"; 
        var cacheResult = _memoryCache.Get<List<QuoteDto>?>(cacheKey);
        if (cacheResult != null)
        {
            return cacheResult;
        }

        try
        {
            if (!await _semaphore.WaitAsync(TimeSpan.FromSeconds(10)))
            {
                return new List<QuoteDto>();
            }

            cacheResult = _memoryCache.Get<List<QuoteDto>?>(cacheKey);
            if (cacheResult != null)
            {
                return cacheResult;
            }

            var quotes = quotesSource switch
            {
                QuotesSource.CoinMarketCap => await GetLatestQuotesFromCoinMarketCapAsync(fromCurrency, toCurrencies),
                QuotesSource.ApiLayer => await GetLatestQuotesFromApiLayerAsync(fromCurrency, toCurrencies),
                _ => throw new ArgumentOutOfRangeException(nameof(quotesSource), quotesSource, null)
            };

            if (quotes is { Count: > 0 })
            {
                //1 minute refresh rate, so no need to make request again inside 1 minute range
                //taking max Timestamp may be not a good idea if we need more relevant data
                //if needed it's possible to store quotes separately, but it's way better to separate reading and writing
                var absoluteExpiration = quotes.Max(q => q.Timestamp) + TimeSpan.FromMinutes(1);
                if (absoluteExpiration > DateTime.UtcNow)
                {
                    _memoryCache.Set(cacheKey, quotes, absoluteExpiration);
                }
            }

            return quotes;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<QuoteDto>> GetLatestQuotesFromCoinMarketCapAsync(string fromCurrency,
        IEnumerable<string> toCurrencies)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var quotesEndpoint = "/v2/cryptocurrency/quotes/latest";
        httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", _settings.CoinMarketCapSettings.ApiKey);

        var quotes = new List<QuoteDto>();
        foreach (var toCurrency in toCurrencies)
        {
            var query = new QueryBuilder
            {
                { "symbol", fromCurrency },
                { "convert", toCurrency }
            };
            var response =
                await httpClient.GetFromJsonAsync<CoinMarketCapQuotesResponse>(
                    $"{_settings.CoinMarketCapSettings.BaseUrl}{quotesEndpoint}{query}");
            if (response?.Status?.ErrorCode != 0)
            {
                continue;
            }

            if (response.Data == null ||
                !response.Data.ContainsKey(fromCurrency) ||
                response.Data[fromCurrency].Length == 0 ||
                response.Data[fromCurrency].First().Quotes == null ||
                response.Data[fromCurrency].First().Quotes.Count == 0)
            {
                continue;
            }
            var quote = response
                .Data[fromCurrency]
                .First()
                .Quotes
                .First();
            quotes.Add(new QuoteDto()
            {
                CurrencyCode = quote.Key,
                Value = quote.Value.Price,
                Timestamp = quote.Value.LastUpdated
            });
        }

        return quotes;
    }

    private async Task<List<QuoteDto>> GetLatestQuotesFromApiLayerAsync(string fromCurrency,
        IEnumerable<string> toCurrencies)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var query = new QueryBuilder
        {
            { "base", fromCurrency },
            { "symbols", string.Join(',', toCurrencies) }
        };
        var quotesEndpoint = "/exchangerates_data/latest";
        httpClient.DefaultRequestHeaders.Add("apikey", _settings.ApiLayerSettings.ApiKey);
        var response =
            await httpClient.GetFromJsonAsync<ApiLayerQuotesResponse>(
                $"{_settings.ApiLayerSettings.BaseUrl}{quotesEndpoint}{query}");
        if (response?.Success != true)
        {
            return new List<QuoteDto>();
        }

        return response
            .Rates
            .Where(q => q.Key != null)
            .Select(q => new QuoteDto()
            {
                CurrencyCode = q.Key,
                Value = q.Value,
                Timestamp = response.Updated
            })
            .ToList();
    }
}