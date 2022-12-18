namespace KnabEngineerRecruitmentTestWebApi.ResponseModels;

public class Quote
{
    public string CurrencyCode { get; set; }
    public decimal Value  { get; set; }
    
    public DateTime Timestamp  { get; set; }
}