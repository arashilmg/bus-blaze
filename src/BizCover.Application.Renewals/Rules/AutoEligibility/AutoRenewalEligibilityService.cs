namespace BizCover.Application.Renewals.Rules.AutoEligibility
{
    public interface IAutoRenewalEligibilityService
    {
        RenewalEligibilityCheckResponse EnsureEligibility(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest);
    }

    public class AutoRenewalEligibilityService : IAutoRenewalEligibilityService
    {
        private readonly IRenewalEligibilityCriteriaFactory _renewalEligibilityCriteriaFactory;

        public AutoRenewalEligibilityService(IRenewalEligibilityCriteriaFactory renewalEligibilityCriteriaFactory)
        {
            _renewalEligibilityCriteriaFactory = renewalEligibilityCriteriaFactory;
        }

        public RenewalEligibilityCheckResponse EnsureEligibility(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest)
        {
            var criterias = _renewalEligibilityCriteriaFactory.GetEligibilityCriterias(new List<EligibilityType>
                {
                    EligibilityType.Both,
                    EligibilityType.Auto
                });

            foreach (var criteria in criterias)
            {
                var response = criteria.IsCriteriaSatisfiedAsync(renewalEligibilityCheckRequest);
                if (!response.IsEligible)
                {
                    return response;
                }
            }

            return new RenewalEligibilityCheckResponse { IsEligible = true};
        }
    }
}
