namespace BizCover.Entity.Renewals;

public class RenewalsEnabled
{
    public bool IsEnabled { get; set; } = true;
    public string? Comments { get; set; }
    public DateTime UpdatedAt { get; set; }
}