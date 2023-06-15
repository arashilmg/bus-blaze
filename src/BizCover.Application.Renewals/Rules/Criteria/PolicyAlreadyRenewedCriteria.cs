using BizCover.Application.Renewals.Helpers;

namespace BizCover.Application.Renewals.Rules.Criteria
{
    [RegisterRenewalEligibilityCriteriaForAttribute(4, EligibilityType.Both)]
    public class PolicyAlreadyRenewedCriteria : IRenewalEligibilityCriteria
    {
        public RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(
            RenewalEligibilityCheckRequest renewalEligibilityCheckRequest) =>
            renewalEligibilityCheckRequest.Renewal.IsPolicyRenewed() == false
                ? new RenewalEligibilityCheckResponse { IsEligible = true}
                : new RenewalEligibilityCheckResponse
                {
                    IsEligible = false,
                    Reason = string.Format(ValidationConstants.PolicyRenewed,
                        renewalEligibilityCheckRequest.Renewal.ExpiringPolicyId,
                        renewalEligibilityCheckRequest.Renewal.RenewedPolicyId.Value)
                };
    }
}
