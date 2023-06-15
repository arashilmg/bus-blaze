using BizCover.Entity.Renewals;

namespace BizCover.Application.Renewals.Rules
{
    public class RenewalEligibilityCheckRequest
    {
        public Renewal Renewal { get; set; }
        public bool HasArrears { get; set; }
    }
}
