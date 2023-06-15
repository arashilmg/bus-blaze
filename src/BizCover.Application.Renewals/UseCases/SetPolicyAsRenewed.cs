using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases;

public class SetPolicyAsRenewed : ISetPolicyAsRenewed
{
    private readonly IRepository<Renewal> _renewalRepository;

    public SetPolicyAsRenewed(IRepository<Renewal> renewalRepository)
    {
        _renewalRepository = renewalRepository;
    }

    public async Task SetAsRenewed(Guid expiringPolicyId, Guid renewedPolicyId, DateTime renewedDate)
    {
        var renewal = await GetRenewalDetailsByExpiringPolicyId(expiringPolicyId);
        renewal.RenewedPolicyId = renewedPolicyId;
        renewal.RenewedDate = renewedDate;
    
        await _renewalRepository.UpdateAsync(renewal.Id, renewal);
        
    }

    private async Task<Renewal> GetRenewalDetailsByExpiringPolicyId(Guid expiringPolicyId) =>
        (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, CancellationToken.None)).First();

    
}
