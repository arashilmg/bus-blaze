using System;

namespace BizCover.Messages.Renewals
{
    public class AutoRenewalOptInFlagUpdatedEvent
    {
        public Guid ExpiringPolicyId { get; set; }
        public string ProductCode { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public DateTime PolicyInceptionDate { get; set; }

        public bool HasSpecialSpecialCircumstances { get; set; }
        public bool AllRenewalsOptIn { get; set; } // AllRenewalsEnabled
        public bool AutoRenewalOptIn { get; set; } // OptIn
        public bool IsProductAllowedToAutoRenew { get; set; }  // CanAutoRenew
        public bool IsPolicyAllowedToAutoRenew { get; set; } // IsEligible
    }
}
