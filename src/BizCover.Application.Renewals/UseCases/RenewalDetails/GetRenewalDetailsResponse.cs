namespace BizCover.Application.Renewals.UseCases.RenewalDetails;

public class GetRenewalDetailsResponse
{
    public Guid ExpiringPolicyId { get; set; }
    public Guid? OrderId { get; set; }
}