using KnabEngineerRecruitmentTestInfrastructure.Models;
using KnabEngineerRecruitmentTestInfrastructure.Models.Dto;

namespace KnabEngineerRecruitmentTestInfrastructure;

public interface IQuotesApiClient
{
    Task<List<QuoteDto>> GetLatestQuotesAsync(QuotesSource quotesSource, string fromCurrency,
        IEnumerable<string> toCurrencies);
}