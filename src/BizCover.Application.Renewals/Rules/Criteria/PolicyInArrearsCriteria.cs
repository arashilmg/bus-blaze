using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria
{
    [RegisterRenewalEligibilityCriteriaForAttribute(7, EligibilityType.Both)]
    public class PolicyInArrearsCriteria : IRenewalEligibilityCriteria
    {
        public PolicyInArrearsCriteria()
        {}

        public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest)
        {
            return renewalEligibilityCheckRequest.HasArrears == false
                ? new RenewalEligibilityCheckResponse { IsEligible = true}
                : new RenewalEligibilityCheckResponse
                {
                    IsEligible = false,
                    Reason = string.Format(ValidationConstants.PolicyInArrears,
                        renewalEligibilityCheckRequest.Renewal.ExpiringPolicyId)
                };
        }
    }
}
