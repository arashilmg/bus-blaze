using BizCover.Entity.Renewals;

namespace BizCover.Application.Renewals.UseCases;

public interface ISetPolicyAsRenewed
{
    Task SetAsRenewed(Guid expiringPolicyId, Guid renewedPolicyId, DateTime renewedPolicyBoundDateTime);
}
