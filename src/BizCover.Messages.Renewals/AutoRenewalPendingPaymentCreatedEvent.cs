using System;

namespace BizCover.Messages.Renewals
{
    public class AutoRenewalPendingPaymentCreatedEvent
    {
        public Guid ExpiringPolicyId { get; set; }
        public Guid OrderId { get; set; }
    }
}
