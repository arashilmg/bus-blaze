namespace BizCover.Application.Renewals.Rules.Criteria
{
    public interface IRenewalEligibilityCriteria
    {
        RenewalEligibilityCheckResponse IsCriteriaSatisfiedAsync(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest);
    }
}
