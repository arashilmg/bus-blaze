namespace BizCover.Entity.Renewals;

public class AutoRenewalEligibility
{
    public bool IsEligible { get; set; }
    public string? Comments { get; set; }
    public DateTime UpdatedAt { get; set; }
}
