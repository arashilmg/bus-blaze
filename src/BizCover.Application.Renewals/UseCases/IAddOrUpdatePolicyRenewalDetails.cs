namespace BizCover.Application.Renewals.UseCases;

public interface IAddOrUpdatePolicyRenewalDetails
{
    Task AddOrUpdate(Guid expiringPolicyId, DateTime policyExpiryDate, DateTime policyInceptionDate,
        string productCode, string status, CancellationToken cancellationToken);
}