using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Renewals;

namespace BizCover.Application.Renewals.UseCases;

public interface IPublishSpecialCircumstancesUpdatedEvent
{
    Task Publish(Guid expiringPolicyId, CancellationToken cancellationToken);
}

public class PublishSpecialCircumstancesUpdatedEvent : IPublishSpecialCircumstancesUpdatedEvent
{
    private readonly IRepository<Renewal> _renewalRepository;
    private readonly IQueuePublisher _queuePublisher;
    private readonly IRenewalConfigService _autoRenewalConfigService;

    public PublishSpecialCircumstancesUpdatedEvent(
        IRepository<Renewal> renewalRepository,
        IQueuePublisher queuePublisher,
        IRenewalConfigService autoRenewalConfigService)
    {
        _renewalRepository = renewalRepository;
        _queuePublisher = queuePublisher;
        _autoRenewalConfigService = autoRenewalConfigService;
    }

    public async Task Publish(Guid expiringPolicyId, CancellationToken cancellationToken)
    {
        var renewal = await GetRenewal(expiringPolicyId, cancellationToken);

        if (renewal != null)
        {

            await _queuePublisher.Publish(new SpecialCircumstancesUpdatedEvent
            {
                ExpiringPolicyId = renewal.ExpiringPolicyId,
                ProductCode = renewal.ProductCode,
                PolicyExpiryDate = renewal.PolicyExpiryDate,
                PolicyInceptionDate = renewal.PolicyInceptionDate,
                HasSpecialSpecialCircumstances = renewal.SpecialCircumstances?.IsApplied ?? false,
                AllRenewalsOptIn = renewal.AllRenewalsEnabled?.IsEnabled ?? false, // AllRenewalsEnabled
                AutoRenewalOptIn = renewal.OptIn, // OptIn
                IsProductAllowedToAutoRenew = _autoRenewalConfigService.CanAutoRenew(renewal.ProductCode, renewal.PolicyInceptionDate),
                IsPolicyAllowedToAutoRenew = renewal.AutoRenewalEligibility?.IsEligible ?? false, // IsEligible
            },
                cancellationToken);
        }
    }

    private async Task<Renewal> GetRenewal(Guid expiringPolicyId, CancellationToken cancellationToken)
        => (await _renewalRepository.FindAsync(
            x => x.PolicyStatus == PolicyStatus.Active
            && x.ExpiringPolicyId == expiringPolicyId
                    && (x.RenewalDates != null && x.RenewalDates.Initiated.HasValue)
                    && x.RenewedPolicyId == null,
            cancellationToken)).SingleOrDefault();
}
