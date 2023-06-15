namespace BizCover.Application.Renewals.Configuration
{
    public abstract class ProductConfig {
        public string ProductCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}

