using System;

namespace BizCover.Messages.Renewals
{
    public class InitiateRenewalCommand
    {
        public Guid ExpiringPolicyId { get; set; }
    }
}
