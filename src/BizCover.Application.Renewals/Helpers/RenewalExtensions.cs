using BizCover.Entity.Renewals;

namespace BizCover.Application.Renewals.Helpers;

public static class RenewalExtensions
{
    public static bool IsPolicyCancelled(this Renewal renewalDto) =>
        renewalDto.PolicyStatus is PolicyStatus.Cancelled;

    public static bool IsPolicyIssued(this Renewal renewalDto) =>
        renewalDto.PolicyStatus is PolicyStatus.Issued;



    public static bool IsPolicyRenewed(this Renewal renewalDto) =>
        renewalDto.RenewedPolicyId.HasValue;

}