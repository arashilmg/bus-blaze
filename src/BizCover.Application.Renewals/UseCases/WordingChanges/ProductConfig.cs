#pragma warning disable CS8618

namespace BizCover.Application.Renewals.UseCases.WordingChanges;

public record ProductConfig
{
    public string ProductCode { get; init; }
    public string Title { get; init; }
    public DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public string Url { get; init; }
}
