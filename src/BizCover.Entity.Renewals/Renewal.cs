using MongoDB.Bson.Serialization.Attributes;

namespace BizCover.Entity.Renewals;

// Old data will have renewal type which we want to ignore as we removed renewal type from Renewal
[BsonIgnoreExtraElements]
public class Renewal : Framework.Domain.Entity
{
    public Guid ExpiringPolicyId { get; set; }
    public string ProductCode { get; set; }
    public string PolicyStatus { get; set; }
    public Guid? OrderId { get; set; }
    public DateTime PolicyExpiryDate { get; set; }
    public DateTime PolicyInceptionDate { get; set; }
    public RenewalDates? RenewalDates { get; set; }
    public AutoRenewalEligibility? AutoRenewalEligibility { get; set; }
    public SpecialCircumstances? SpecialCircumstances { get; set; }
    public RenewalsEnabled? AllRenewalsEnabled { get; set; }
    public bool OptIn { get; set; }
    public bool HasArrears { get; set; }
    public Guid? RenewedPolicyId { get; set; }
    public DateTime? RenewedDate { get; set; }
}
