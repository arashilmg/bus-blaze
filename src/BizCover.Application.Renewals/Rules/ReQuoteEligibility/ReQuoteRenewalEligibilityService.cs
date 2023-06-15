namespace BizCover.Application.Renewals.Rules.ReQuoteEligibility
{
    public interface IReQuoteRenewalEligibilityService
    {
        RenewalEligibilityCheckResponse EnsureEligibility(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest);
    }

    public class ReQuoteRenewalEligibilityService : IReQuoteRenewalEligibilityService
    {
        private readonly IRenewalEligibilityCriteriaFactory _renewalEligibilityCriteriaFactory;

        public ReQuoteRenewalEligibilityService(IRenewalEligibilityCriteriaFactory renewalEligibilityCriteriaFactory)
        {
            _renewalEligibilityCriteriaFactory = renewalEligibilityCriteriaFactory;
        }

        public RenewalEligibilityCheckResponse EnsureEligibility(RenewalEligibilityCheckRequest renewalEligibilityCheckRequest)
        {
            var criterias = _renewalEligibilityCriteriaFactory.GetEligibilityCriterias(new List<EligibilityType>
            {
                EligibilityType.Both,
                EligibilityType.ReQuote
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
