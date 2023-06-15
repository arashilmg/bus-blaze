using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria
{
    [RegisterRenewalEligibilityCriteriaForAttribute(2, EligibilityType.Both)]
    public class PolicyCancelledCriteria : IRenewalEligibilityCriteria
    {
        public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(
            RenewalEligibilityCheckRequest renewalEligibilityCheckRequest) =>
            renewalEligibilityCheckRequest.Renewal.IsPolicyCancelled()
                ? new RenewalEligibilityCheckResponse
                {
                    IsEligible = false,
                    Reason = string.Format(ValidationConstants.PolicyIsNotActive,
                        renewalEligibilityCheckRequest.Renewal.ExpiringPolicyId,
                        renewalEligibilityCheckRequest.Renewal.PolicyStatus)
                }
                : new RenewalEligibilityCheckResponse { IsEligible = true};
    }
}
