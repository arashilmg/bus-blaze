namespace BizCover.Application.Renewals.Rules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RegisterRenewalEligibilityCriteriaForAttribute : Attribute
    {
        public int OrderOfCriteriaCheck { get; }
        public EligibilityType EligibilityType { get; set; }

        public RegisterRenewalEligibilityCriteriaForAttribute(
            int orderOfCriteriaCheck,
            EligibilityType eligibilityType)
        {
            OrderOfCriteriaCheck = orderOfCriteriaCheck;
            EligibilityType = eligibilityType;
        }
    }
}
