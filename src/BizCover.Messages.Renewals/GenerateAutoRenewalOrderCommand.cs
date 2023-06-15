using System;

namespace BizCover.Messages.Renewals
{
    public class GenerateAutoRenewalOrderCommand
    {
        public Guid ExpiringPolicyId { get; set; }
    }
}
