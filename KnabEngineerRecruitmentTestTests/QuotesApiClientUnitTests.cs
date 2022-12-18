using System.Net;
using KnabEngineerRecruitmentTestInfrastructure;
using KnabEngineerRecruitmentTestInfrastructure.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace KnabEngineerRecruitmentTestTests;

public class QuotesApiClientUnitTests
{
    private readonly QuotesApiClient _client;

    public QuotesApiClientUnitTests()
    {
        var apiSettings = new ExternalQuotesApiSettings()
        {
            ApiLayerSettings = new ExternalApiSettings()
            {
                ApiKey = Guid.NewGuid().ToString(),
                BaseUrl = "http://ApiLayer.mock"
            },
            CoinMarketCapSettings = new ExternalApiSettings()
            {
                ApiKey = Guid.NewGuid().ToString(),
                BaseUrl = "http://CoinMarketCap.mock"
            }
        };
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(factory => factory.CreateClient(""))
            .Returns(() => new HttpClient(new HttpMessageHandlerMock(apiSettings)));
        var settings = new OptionsWrapper<ExternalQuotesApiSettings>(apiSettings);
        var cacheMock = new Mock<IMemoryCache>();
        cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>()))
            .Returns(() => new Mock<ICacheEntry>().Object);
        _client = new QuotesApiClient(httpClientFactoryMock.Object, settings, cacheMock.Object);
    }

    [Theory]
    [InlineData(QuotesSource.CoinMarketCap)]
    [InlineData(QuotesSource.ApiLayer)]
    public async Task Should_ParseResponse_ReturnAllQuotes(QuotesSource quotesSource)
    {
        var toCurrencies = new[] { "USD", "EUR", "BRL", "GBP", "AUD" };
        var result = await _client.GetLatestQuotesAsync(quotesSource, "BTC", toCurrencies);
        Assert.NotNull(result);
        Assert.Equal(toCurrencies.Length, result.Count);
    }

    [Theory]
    [InlineData(QuotesSource.CoinMarketCap)]
    [InlineData(QuotesSource.ApiLayer)]
    public async Task Should_HandleEmptyResult_ReturnNoQuotes(QuotesSource quotesSource)
    {
        var toCurrencies = new[] { "USD", "EUR", "BRL", "GBP", "AUD" };
        var result = await _client.GetLatestQuotesAsync(quotesSource, "Unknown", toCurrencies);
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
    }

    [Theory]
    [InlineData(QuotesSource.CoinMarketCap)]
    [InlineData(QuotesSource.ApiLayer)]
    public async Task Should_HandleServerExceptionResult_ThrowException(QuotesSource quotesSource)
    {
        var toCurrencies = new[] { "USD", "EUR", "BRL", "GBP", "AUD" };
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _client.GetLatestQuotesAsync(quotesSource, "InternalServerError", toCurrencies));
    }

    private class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly ExternalQuotesApiSettings _apiSettings;

        public HttpMessageHandlerMock(ExternalQuotesApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            const string apiLayerKeyHeader = "apikey";
            const string coinMarketCapApiKeyHeader = "X-CMC_PRO_API_KEY";
            if (request.RequestUri.Query.Contains("Unknown"))
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent("{}"),
                    StatusCode = HttpStatusCode.OK
                });
            }

            if (request.RequestUri.Query.Contains("InternalServerError"))
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent(""),
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }

            if (request.Headers.Contains(coinMarketCapApiKeyHeader))
            {
                var authorizationHeaderValues = request.Headers.GetValues(coinMarketCapApiKeyHeader).ToList();
                if (authorizationHeaderValues.Count() > 1 ||
                    authorizationHeaderValues.First() != _apiSettings.CoinMarketCapSettings.ApiKey)
                {
                    throw new UnauthorizedAccessException("Value of X-CMC_PRO_API_KEY is not correct.");
                }


                var realLifeResponse = File.ReadAllText("CoinMarketCapQuoteResponse.json");
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent(realLifeResponse),
                    StatusCode = HttpStatusCode.OK
                });
            }

            if (request.Headers.Contains(apiLayerKeyHeader))
            {
                var authorizationHeaderValues = request.Headers.GetValues(apiLayerKeyHeader).ToList();
                if (authorizationHeaderValues.Count() > 1 ||
                    authorizationHeaderValues.First() != _apiSettings.ApiLayerSettings.ApiKey)
                {
                    throw new UnauthorizedAccessException("Value of apikey is not correct.");
                }

                var realLifeResponse = File.ReadAllText("ApiLayerResponse.json");
                return Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent(realLifeResponse),
                    StatusCode = HttpStatusCode.OK
                });
            }

            throw new NotImplementedException("HttpMessageHandlerMock does not cover this case.");
        }
    }
}