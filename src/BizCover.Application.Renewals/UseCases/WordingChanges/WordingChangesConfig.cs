namespace BizCover.Application.Renewals.UseCases.WordingChanges;

public record WordingChangesConfig
{
    public IEnumerable<ProductConfig>? Products { get; init; }

    public string GetWordingConfigUrl(string productCode, DateTime effectiveDate)
    {
        var productConfig = Products?.FirstOrDefault(p =>
            p.ProductCode == productCode.ToUpperInvariant()
            && p.EffectiveFrom.Date <= effectiveDate.Date
            && (p.EffectiveTo == null || p.EffectiveTo.Value.Date >= effectiveDate.Date));

        return productConfig?.Url ?? string.Empty;
    }
}
