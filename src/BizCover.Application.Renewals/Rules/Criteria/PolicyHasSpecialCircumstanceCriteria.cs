using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria
{
    [RegisterRenewalEligibilityCriteriaForAttribute(3, EligibilityType.Both)]
    public class PolicyHasSpecialCircumstanceCriteria : IRenewalEligibilityCriteria
    {
        public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(
            RenewalEligibilityCheckRequest renewalEligibilityCheckRequest) =>
            renewalEligibilityCheckRequest.Renewal?.SpecialCircumstances?.IsApplied == true
                ? new RenewalEligibilityCheckResponse
                {
                    IsEligible = false,
                    Reason = string.Format(ValidationConstants.SpecialCircumstance,
                        renewalEligibilityCheckRequest.Renewal.ExpiringPolicyId)
                }
                : new RenewalEligibilityCheckResponse { IsEligible = true,};
    }
}


