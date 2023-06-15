namespace BizCover.Application.Renewals.DTO;

public class AutoRenewalEligibilityDto
{
    public bool IsEligible { get; init; }
    public string? Comments { get; init; }
    public DateTime UpdatedAt { get; init; }
}
