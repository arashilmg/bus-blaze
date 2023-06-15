using System;
using System.Collections.Generic;

namespace BizCover.Messages.Renewals
{
    public class RenewalOrderGeneratedEvent
    {
        public Guid ExpiringPolicyId { get; set; }
        public string ExpiringPolicyNumber { get; set; }
        public DateTime ExpiringPolicyExpiryDate { get; set; }
        public Guid ContactId { get; set; }
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPremium { get; set; }
        public string RenewalType { get; set; }
        public IEnumerable<Offer> Offers { get; set; }
    }

    public class Offer
    {
        public Guid OfferId { get; set; }

        public DateTime ExpiryDate { get; set; }

        public IEnumerable<ProductType> ProductTypes { get; set; }

        public bool Sold { get; set; }

        public bool Expired { get; set; }

        public Guid ContactId { get; set; }

        public int Code { get; set; }
    }

    public class ProductType
    {
        public string ProductTypeCode { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
