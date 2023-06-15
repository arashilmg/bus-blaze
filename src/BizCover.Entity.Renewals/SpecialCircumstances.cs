namespace BizCover.Entity.Renewals;

public class SpecialCircumstances
{
    public bool IsApplied { get; set; }
    public string? Comments { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Reason { get; set; }
    public string? SecondLevelReason { get; set; }
}
