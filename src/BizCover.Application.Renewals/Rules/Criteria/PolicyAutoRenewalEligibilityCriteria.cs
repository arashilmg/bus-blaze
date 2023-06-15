using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria;

[RegisterRenewalEligibilityCriteriaFor(6, EligibilityType.Auto)]
public class PolicyAutoRenewalEligibilityCriteria : IRenewalEligibilityCriteria
{
    public PolicyAutoRenewalEligibilityCriteria()
    { }

    public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest)
    {
        return renewalEligibilityCheckRequest.Renewal.AutoRenewalEligibility!.IsEligible
            ? new RenewalEligibilityCheckResponse { IsEligible = true}
            : new RenewalEligibilityCheckResponse
            {
                IsEligible = false,
                Reason = string.Format(ValidationConstants.PolicyAutoUnEligible,
                    renewalEligibilityCheckRequest.Renewal.ExpiringPolicyId,
                    renewalEligibilityCheckRequest.Renewal.AutoRenewalEligibility!.Comments)
            };
    }
}