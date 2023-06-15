using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;

namespace BizCover.Application.Renewals.Rules.Criteria;

[RegisterRenewalEligibilityCriteriaFor(5, EligibilityType.Auto)]
public class ProductAutoRenewalEligibilityCriteria : IRenewalEligibilityCriteria
{
    private readonly IRenewalConfigService _renewalConfigService;
    public ProductAutoRenewalEligibilityCriteria(IRenewalConfigService renewalConfigService)
    {
        _renewalConfigService = renewalConfigService;
    }

    public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest)
    {
        var canAutoRenew = _renewalConfigService.CanAutoRenew(renewalEligibilityCheckRequest.Renewal.ProductCode, renewalEligibilityCheckRequest.Renewal.PolicyInceptionDate);

        return canAutoRenew
            ? new RenewalEligibilityCheckResponse { IsEligible = true}
            : new RenewalEligibilityCheckResponse
            {
                IsEligible = false,
                Reason = string.Format(ValidationConstants.ProductAutoUnEligible,
                    renewalEligibilityCheckRequest.Renewal.ProductCode)
            };
    }
}