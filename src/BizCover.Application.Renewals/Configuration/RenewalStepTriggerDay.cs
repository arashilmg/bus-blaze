namespace BizCover.Application.Renewals.Configuration;

public class RenewalStepTriggerDay : ProductConfig
{
    public int Initiation { get; set; }
    public int OrderGeneration { get; set; }
    public int OrderSubmission { get; set; }
}