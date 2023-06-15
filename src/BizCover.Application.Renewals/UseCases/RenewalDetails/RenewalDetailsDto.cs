using BizCover.Application.Renewals.DTO;

namespace BizCover.Application.Renewals.UseCases.RenewalDetails;

public class RenewalDetailsDto
{
    public Guid ExpiringPolicyId { get; init; }
    public string ProductCode { get; set; }
    public string PolicyStatus { get; set; }
    public Guid? OrderId { get; init; }
    public DateTime PolicyExpiryDate { get; init; }
    public DateTime PolicyInceptionDate { get; init; }
    public RenewalDatesDto? RenewalDates { get; set; }
    public AutoRenewalEligibilityDto? AutoRenewalEligibility { get; set; }
    public SpecialCircumstancesDto? SpecialCircumstances { get; set; }
    public bool OptIn { get; set; }
    public bool HasArrears { get; set; }
    public string? RenewalType { get; set; }
    public Guid? RenewedPolicyId { get; set; }
    public DateTime? RenewedDate { get; set; }

    public bool AllEnabledFlag { get; set; }
}
