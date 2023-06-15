using System;

namespace BizCover.Messages.Renewals
{
    public class PolicyRenewedEvent
    {
        public Guid ExpiringPolicyId { get; set; }
        public Guid RenewedPolicyId { get; set; }
        public DateTime RenewalDate { get; set; }
    }
}
