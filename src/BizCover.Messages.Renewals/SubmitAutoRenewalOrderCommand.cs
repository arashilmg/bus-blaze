using System;

namespace BizCover.Messages.Renewals
{
    public class SubmitAutoRenewalOrderCommand
    {
        public Guid ExpiringPolicyId { get; set; }
        public Guid OrderId { get; set; }
    }
}
