using BizCover.Application.Renewals.Rules.Criteria;

namespace BizCover.Application.Renewals.Rules
{
    public interface IRenewalEligibilityCriteriaFactory
    {
        List<IRenewalEligibilityCriteria> GetEligibilityCriterias(IList<EligibilityType> eligibilityTypes);
    }

    public class RenewalEligibilityCriteriaFactory : IRenewalEligibilityCriteriaFactory
    {
        private readonly IEnumerable<IRenewalEligibilityCriteria> _agentRenewalEligibilityCriteria;

        public RenewalEligibilityCriteriaFactory(IEnumerable<IRenewalEligibilityCriteria> agentRenewalEligibilityCriteria)
        {
            _agentRenewalEligibilityCriteria = agentRenewalEligibilityCriteria;
        }

        public List<IRenewalEligibilityCriteria> GetEligibilityCriterias(IList<EligibilityType> eligibilityTypes)
        {
            var matchingCriteriaByOrder = new List<(IRenewalEligibilityCriteria criteria, int order)>();

            foreach (var criteria in _agentRenewalEligibilityCriteria)
            {
                var attributes = criteria
                    .GetType()
                    .GetCustomAttributes(typeof(RegisterRenewalEligibilityCriteriaForAttribute), false);

                foreach (RegisterRenewalEligibilityCriteriaForAttribute attribute in attributes)
                {
                    if (eligibilityTypes.Contains(attribute.EligibilityType))
                    {
                        matchingCriteriaByOrder.Add((criteria, attribute.OrderOfCriteriaCheck));
                    }
                }
            }

            return matchingCriteriaByOrder
                .OrderBy(x => x.order)
                .Select(c => c.criteria)
                .ToList();
        }
    }
}
