namespace KnabEngineerRecruitmentTestInfrastructure.Models.CoinMarketCap;

public class CoinMarketCapQuotesResponse
{
    public Status Status { get; set; }
    public Dictionary<string, Currency[]> Data { get; set; }
}