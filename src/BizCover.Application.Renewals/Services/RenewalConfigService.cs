using BizCover.Application.Renewals.Configuration;
using BizCover.Application.Renewals.Exception;


namespace BizCover.Application.Renewals.Services
{
    internal class RenewalConfigService : IRenewalConfigService
    {
        private readonly CanAutoRenewProductConfig _canAutoRenewProductConfig;
        private readonly RenewalStepTriggerDayConfig _autoRenewalStepTriggerDayConfig;

        public RenewalConfigService(CanAutoRenewProductConfig canAutoRenewProductConfig,
            RenewalStepTriggerDayConfig autoRenewalStepTriggerDayConfig)
        {
            _canAutoRenewProductConfig = canAutoRenewProductConfig;
            _autoRenewalStepTriggerDayConfig = autoRenewalStepTriggerDayConfig;
        }

        public bool CanAutoRenew(string productCode, DateTime effectiveDate)
        {
            var productConfigs = _canAutoRenewProductConfig.Products.Where(x => x.ProductCode == productCode).ToList();
            if (!productConfigs.Any())
            {
                throw new ProductConfigMissingException($"{nameof(CanAutoRenewProductConfig)} not found for {productCode}");
            }

            var productConfig = productConfigs.SingleOrDefault(x => x.EffectiveFrom.Date <= effectiveDate.Date && (x.EffectiveTo == null || x.EffectiveTo.Value.Date >= effectiveDate.Date));
            if (productConfig == null)
            {
                throw new ProductConfigMissingException($"{nameof(CanAutoRenewProductConfig)} not found for {productCode} with date {effectiveDate}");
            }

            return productConfig.CanAutoRenew;
        }

        public RenewalStepTriggerDay GetRenewalStepTriggerDay(string productCode, DateTime effectiveDate)
        {
            var productConfigs = _autoRenewalStepTriggerDayConfig.Products.Where(x => x.ProductCode == productCode).ToList();

            if (!productConfigs.Any())
            {
                throw new ProductConfigMissingException($"{nameof(RenewalStepTriggerDayConfig)} not found for {productCode}");
            }

            var productConfig = productConfigs.SingleOrDefault(x => x.EffectiveFrom.Date <= effectiveDate.Date &&
                                              (x.EffectiveTo == null || x.EffectiveTo.Value.Date >= effectiveDate.Date));

            return productConfig ?? throw new ProductConfigMissingException(
                $"{nameof(RenewalStepTriggerDayConfig)} not found for {productCode} with date {effectiveDate}");
        }
    }
}



