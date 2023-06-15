using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases.WordingChanges;
using BizCover.Entity.Renewals;
using BizCover.Messages.Renewals;

namespace BizCover.Application.Renewals.UseCases
{
    public class InitiateRenewal
    {
        private readonly IQueuePublisher _queuePublisher;
        private readonly IRenewalService _renewalService;
        private readonly IRenewalConfigService _autoRenewalConfigService;
        private readonly WordingChangesConfig _wordingChangesConfig;

        public InitiateRenewal(IQueuePublisher queuePublisher, 
            IRenewalService renewalService, 
            IRenewalConfigService autoRenewalConfigService, 
            WordingChangesConfig wordingChangesConfig)
        {
            _queuePublisher = queuePublisher;
            _renewalService = renewalService;
            _autoRenewalConfigService = autoRenewalConfigService;
            _wordingChangesConfig = wordingChangesConfig;
        }

        public async Task Initiate(Guid expiringPolicyId, CancellationToken cancellationToken)
        {
            var hasArrears = await _renewalService.HasArrears(expiringPolicyId, cancellationToken);
            var renewal = await _renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, cancellationToken);
            
            await PublishRenewalInitializedEvent(expiringPolicyId, renewal, hasArrears, cancellationToken);
            await _renewalService.SetInitiationDetails(expiringPolicyId, cancellationToken);
        }

        private async Task PublishRenewalInitializedEvent(Guid expiringPolicyId, Renewal renewal, 
            bool hasArrears, CancellationToken cancellationToken)
        {
            var renewalInitializedEvent = new RenewalInitializedEvent
            {
                ExpiringPolicyId = expiringPolicyId,
                IsProductAllowedToAutoRenew = _autoRenewalConfigService.CanAutoRenew(renewal.ProductCode, renewal.PolicyInceptionDate),
                IsPolicyAllowedToAutoRenew = renewal.AutoRenewalEligibility?.IsEligible ?? default,
                AutoRenewalOptIn = renewal.OptIn,
                HasArrears = hasArrears,
                HasSpecialSpecialCircumstances = renewal.SpecialCircumstances?.IsApplied ?? false,
                AllRenewalsOptIn = renewal.AllRenewalsEnabled?.IsEnabled ?? true,
                WordingChangeDocUrl = _wordingChangesConfig.GetWordingConfigUrl(renewal.ProductCode, renewal.PolicyExpiryDate)
            };

            await _queuePublisher.Publish(renewalInitializedEvent, cancellationToken);
        }
    }
}
