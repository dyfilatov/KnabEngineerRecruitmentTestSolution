using KnabEngineerRecruitmentTestInfrastructure;
using KnabEngineerRecruitmentTestInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quote = KnabEngineerRecruitmentTestWebApi.ResponseModels.Quote;

namespace KnabEngineerRecruitmentTestWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuotesApiClient _quotesApiClient;
    private readonly ILogger<QuotesController> _logger;
    private readonly List<string> _defaultCurrenciesQuotes;
    private readonly QuotesSource _defaultQuoteSource;

    public QuotesController(IQuotesApiClient quotesApiClient, IOptions<AppSettings> appSettings, ILogger<QuotesController> logger)
    {
        _quotesApiClient = quotesApiClient;
        _logger = logger;
        _defaultCurrenciesQuotes = appSettings.Value.DefaultCurrenciesQuotes;
        _defaultQuoteSource = appSettings.Value.DefaultQuotesSource;
    }

    [HttpGet]
    public async Task<ActionResult<List<Quote>?>> Get(string currencyCode = "BTC")
    {
        try
        {
            var result = await _quotesApiClient.GetLatestQuotesAsync(_defaultQuoteSource,
                currencyCode, _defaultCurrenciesQuotes);

            return result.Select(c => new Quote()
            {
                CurrencyCode = c.CurrencyCode,
                Value = c.Value,
                Timestamp = c.Timestamp
            }).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get quotes error.");
            return Problem("Couldn't get data, reason unknown", null, 500);
        }
    }
}