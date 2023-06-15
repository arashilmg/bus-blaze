namespace BizCover.Application.Renewals.Rules;

public class RenewalEligibilityCheckResponse
{
    public bool IsEligible { get; set; }
    public string Reason { get; set; }
}