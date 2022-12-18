using System.Text.Json.Serialization;

namespace KnabEngineerRecruitmentTestInfrastructure.Models.CoinMarketCap;

public class Quote
{
    public decimal Price { get; set; }
    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}