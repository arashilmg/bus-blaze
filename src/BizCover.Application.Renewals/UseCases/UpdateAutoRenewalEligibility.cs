using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases;

public class UpdateAutoRenewalEligibility
{
    private readonly IRepository<Renewal> _renewalRepository;

    public UpdateAutoRenewalEligibility(IRepository<Renewal> renewalRepository)
    {
        _renewalRepository = renewalRepository;
    }

    public async Task Update(
        Guid expiringPolicyId, bool isEligible, string comments, CancellationToken cancellationToken)
    {
        var renewal = (await _renewalRepository
            .FindAsync(r => r.ExpiringPolicyId == expiringPolicyId, cancellationToken))
            .SingleOrDefault();

        if (renewal == null)
        {
            throw new NotFoundException<Renewal>(expiringPolicyId);
        }


        if (renewal.SpecialCircumstances?.IsApplied == true)
        {
            throw new InvalidOperationException($"Cant set autorenewal flag when special circumstances apply policy{expiringPolicyId}");
        }

        var currentDateTime = DateTime.Now.ToUniversalTime();
        renewal.AutoRenewalEligibility ??= new Entity.Renewals.AutoRenewalEligibility();
        renewal.AutoRenewalEligibility.IsEligible = isEligible;
        renewal.AutoRenewalEligibility.Comments = comments;
        renewal.AutoRenewalEligibility.UpdatedAt = currentDateTime;

        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
    }
}
