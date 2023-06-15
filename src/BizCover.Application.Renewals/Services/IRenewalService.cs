using BizCover.Entity.Renewals;

namespace BizCover.Application.Renewals.Services
{
    public interface IRenewalService
    {
        Task<Renewal> GetRenewalDetailsByOrderId(Guid orderId, CancellationToken cancellationToken);
        Task<Renewal> GetRenewalDetailsForExpiringPolicy(Guid expiringPolicyId, CancellationToken cancellationToken);
        Task<bool> HasArrears(Guid expiringPolicyId, CancellationToken cancellationToken);
        Task SetInitiationDetails(Guid expiringPolicyId, CancellationToken cancellationToken);
        Task SetOrderGeneratedDetails(Guid expiringPolicyId, Guid orderId, CancellationToken cancellationToken);
        Task SetOrderSubmissionDetails(Guid expiringPolicyId, CancellationToken cancellationToken);
        Task<Renewal> GetRenewalDetailsForRenewedPolicy(Guid renewedPolicyId, CancellationToken cancellationToken);
        Task UpdateAllRenewalsFlagToTrueFromNull(CancellationToken cancellationToken);
    }
}
