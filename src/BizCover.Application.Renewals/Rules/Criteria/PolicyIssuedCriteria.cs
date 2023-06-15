using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria
{
    [RegisterRenewalEligibilityCriteriaForAttribute(1,EligibilityType.Both)]
    public class PolicyIssuedCriteria : IRenewalEligibilityCriteria
    {
        public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(
            RenewalEligibilityCheckRequest renewalEligibilityCheckRequest) =>
            renewalEligibilityCheckRequest.Renewal.IsPolicyIssued()
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
