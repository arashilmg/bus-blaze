using BizCover.Application.Offers;

namespace BizCover.Application.Renewals.UseCases.GenerateRenewalOrder;

public class ResultDto
{
    public Guid OrderId { get; set; }
    public string PolicyNumber { get; set; }
    public DateTime PolicyExpiryDate { get; set; }
    public string ContactId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPremium { get; set; }
    public string FailedReason { get; set; }
    public IEnumerable<OfferDto> Offers { get; set; }
}
