namespace BizCover.Application.Renewals.DTO;

public class EnableRenewalModel
{
    public bool IsEnabled { get; init; }
    public string? Comments { get; init; }
    public DateTime UpdatedAt { get; init; }
}
