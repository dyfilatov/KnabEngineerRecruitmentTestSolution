namespace KnabEngineerRecruitmentTestInfrastructure.Models.Dto;

public class QuoteDto
{
    public string CurrencyCode { get; set; }
    public decimal Value  { get; set; }
    
    public DateTime Timestamp { get; set; }
}