using BizCover.Application.Renewals.Helpers;
using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases
{
    public class AddOrUpdatePolicyRenewalDetails : IAddOrUpdatePolicyRenewalDetails
    {
        private readonly IRepository<Renewal> _renewalRepository;
        private readonly IRenewalConfigService _autoRenewalConfigService;

        public AddOrUpdatePolicyRenewalDetails(IRepository<Renewal> renewalRepository,
            IRenewalConfigService autoRenewalConfigService)
        {
            _renewalRepository = renewalRepository;
            _autoRenewalConfigService = autoRenewalConfigService;
        }

        public async Task AddOrUpdate(Guid expiringPolicyId, DateTime policyExpiryDate, DateTime policyInceptionDate,
            string productCode, string status, CancellationToken cancellationToken)
        {
            var autoRenewalStepTriggerDay =
                _autoRenewalConfigService.GetRenewalStepTriggerDay(productCode, policyInceptionDate);

            var renewal = new Renewal
            {
                ExpiringPolicyId = expiringPolicyId,
                PolicyExpiryDate = policyExpiryDate,
                ProductCode = productCode,
                PolicyStatus = status,
                PolicyInceptionDate = policyInceptionDate,
                RenewalDates = new RenewalDates
                {
                    Initiation = policyExpiryDate.CalculateDate(autoRenewalStepTriggerDay.Initiation),
                    OrderGeneration = policyExpiryDate.CalculateDate(autoRenewalStepTriggerDay.OrderGeneration),
                    OrderSubmission = policyExpiryDate.CalculateDate(autoRenewalStepTriggerDay.OrderSubmission)
                },
                AutoRenewalEligibility = new AutoRenewalEligibility
                {
                    IsEligible = true,
                    UpdatedAt = DateTime.Now.ToUniversalTime()
                },
                SpecialCircumstances = new SpecialCircumstances
                {
                    IsApplied = false,
                    UpdatedAt = DateTime.UtcNow
                },
                AllRenewalsEnabled = new RenewalsEnabled()
                {
                    IsEnabled = true,
                    UpdatedAt = DateTime.UtcNow
                },                
                OptIn = true,
                HasArrears = false
            };

            await UpsertEntity(renewal, cancellationToken);
        }

        private async Task<Renewal?> FindByExpiringPolicyId(Guid expiringPolicyId, CancellationToken cancellationToken) =>
            (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken)).FirstOrDefault();

        private async Task UpsertEntity(Renewal renewal, CancellationToken cancellationToken)
        {
            var entity = await FindByExpiringPolicyId(renewal.ExpiringPolicyId, cancellationToken);
            if (entity == null)
            {
                await _renewalRepository.AddOneAsync(renewal, cancellationToken);
            }
            else
            {
                renewal.Id = entity.Id;
                await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
            }
        }
    }
}
