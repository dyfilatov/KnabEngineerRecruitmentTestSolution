namespace KnabEngineerRecruitmentTestInfrastructure.Models;

/// <summary>
/// Reponse from api.apilayer.com (exchangeratesapi.io)
/// </summary>
public class ApiLayerQuotesResponse
{
    public Dictionary<string, decimal> Rates { get; set; }
    public bool Success { get; set; }
    public int Timestamp { get; set; }
    public DateTime Updated => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;
}