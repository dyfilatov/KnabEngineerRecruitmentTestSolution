using KnabEngineerRecruitmentTestInfrastructure.Models;

namespace KnabEngineerRecruitmentTestWebApi;

public class AppSettings
{
    public List<string> DefaultCurrenciesQuotes { get; set; }
    public QuotesSource DefaultQuotesSource { get; set; }
}