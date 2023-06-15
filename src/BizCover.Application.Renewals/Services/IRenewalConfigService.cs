using BizCover.Application.Renewals.Configuration;

namespace BizCover.Application.Renewals.Services;

public interface IRenewalConfigService
{
    bool CanAutoRenew(string productCode, DateTime effectiveDate);
    RenewalStepTriggerDay GetRenewalStepTriggerDay(string productCode, DateTime effectiveDate);
}