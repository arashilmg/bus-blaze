namespace BizCover.Application.Renewals.DTO;

public class SpecialCircumstancesDto
{
    public bool IsApplied { get; init; }
    public string? Comments { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string Reason { get; internal set; }
    public string SecondLevelReason { get; internal set; }
}
