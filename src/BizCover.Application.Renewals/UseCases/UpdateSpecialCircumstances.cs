using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases;

public class UpdateSpecialCircumstances
{
    private readonly IRepository<Renewal> _renewalRepository;
    private readonly IPublishSpecialCircumstancesUpdatedEvent _publishSpecialCircumstancesUpdatedEvent;

    public UpdateSpecialCircumstances(IRepository<Renewal> renewalRepository, IPublishSpecialCircumstancesUpdatedEvent publishSpecialCircumstancesUpdatedEvent)
    {
        _renewalRepository = renewalRepository;
        _publishSpecialCircumstancesUpdatedEvent = publishSpecialCircumstancesUpdatedEvent;
    }

    public async Task Update(
        Guid expiringPolicyId, bool isApplied, string comments, string reason, string secondLevelReason, CancellationToken cancellationToken)
    {
        var renewal = (await _renewalRepository
            .FindAsync(r => r.ExpiringPolicyId == expiringPolicyId, cancellationToken))
            .SingleOrDefault();

        if (renewal == null)
        {
            throw new NotFoundException<Renewal>(expiringPolicyId);
        }

        renewal.SpecialCircumstances ??= new SpecialCircumstances();
        renewal.SpecialCircumstances.IsApplied = isApplied;
        renewal.SpecialCircumstances.Comments = comments;
        renewal.SpecialCircumstances.UpdatedAt = DateTime.UtcNow;
        renewal.SpecialCircumstances.Reason = reason;
        renewal.SpecialCircumstances.SecondLevelReason = secondLevelReason;

        await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
        await _publishSpecialCircumstancesUpdatedEvent.Publish(expiringPolicyId, cancellationToken);
    }
}
