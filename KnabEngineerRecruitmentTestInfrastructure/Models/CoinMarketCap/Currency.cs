using System.Text.Json.Serialization;

namespace KnabEngineerRecruitmentTestInfrastructure.Models.CoinMarketCap;

public class Currency
{
    [JsonPropertyName("Quote")]
    public Dictionary<string , Quote> Quotes { get; set; }
}