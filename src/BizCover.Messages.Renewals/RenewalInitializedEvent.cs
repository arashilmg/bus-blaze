using System;

namespace BizCover.Messages.Renewals
{
    public class RenewalInitializedEvent
    {
        public Guid ExpiringPolicyId { get; set; }
        public bool HasSpecialSpecialCircumstances { get; set; }
        public bool AllRenewalsOptIn { get; set; } // Previously AllRenewalsEnabled
        public bool AutoRenewalOptIn { get; set; } // Previously OptIn
        public bool HasArrears { get; set; }
        public bool IsProductAllowedToAutoRenew { get; set; }  // Previously CanAutoRenew
        public bool IsPolicyAllowedToAutoRenew { get; set; } // Previously IsEligible
        public string WordingChangeDocUrl { get; set; }
    }
}
