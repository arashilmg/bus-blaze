using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Renewals;

namespace BizCover.Application.Renewals.UseCases;

public class PublishDueForRenewalEvents
{
    private readonly IRepository<Renewal> _renewalRepository;
    private readonly IQueuePublisher _queuePublisher;
    private readonly IRenewalConfigService _autoRenewalConfigService;

    public PublishDueForRenewalEvents(
        IRepository<Renewal> renewalRepository, 
        IQueuePublisher queuePublisher, 
        IRenewalConfigService autoRenewalConfigService)
    {
        _renewalRepository = renewalRepository;
        _queuePublisher = queuePublisher;
        _autoRenewalConfigService = autoRenewalConfigService;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var renewals = await GetRenewals(cancellationToken);

        foreach (var renewal in renewals)
        {
            await _queuePublisher.Publish(new DueForRenewalEvent
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

    private async Task<IEnumerable<Renewal>> GetRenewals(CancellationToken cancellationToken)
    {
        return await _renewalRepository.FindAsync(
            x => x.PolicyStatus == PolicyStatus.Active
                    && (x.RenewalDates != null && x.RenewalDates.Initiated.HasValue)
                    && x.RenewedPolicyId == null,
            cancellationToken);
    }
}
