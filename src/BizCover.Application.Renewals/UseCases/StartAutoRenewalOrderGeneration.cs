using BizCover.Application.Renewals.Helpers;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Renewals;

namespace BizCover.Application.Renewals.UseCases
{
    public class StartAutoRenewalOrderGeneration
    {
        private readonly IQueuePublisher _queuePublisher;
        private readonly IRepository<Renewal> _renewalRepository;

        public StartAutoRenewalOrderGeneration(IQueuePublisher queuePublisher,
            IRepository<Renewal> renewalRepository)
        {
            _queuePublisher = queuePublisher;
            _renewalRepository = renewalRepository;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var renewals = await GetRenewals(cancellationToken);

            foreach (var renewal in renewals)
            {
                await _queuePublisher.Send(new GenerateAutoRenewalOrderCommand { ExpiringPolicyId = renewal.ExpiringPolicyId }, cancellationToken);
            }
        }

        private async Task<IEnumerable<Renewal>> GetRenewals(CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(1);

            return await _renewalRepository.FindAsync(
                x => x.RenewalDates!.OrderGeneration >= startDate
                     && x.RenewalDates.OrderGeneration < endDate
                     && x.OptIn
                     && (x.SpecialCircumstances == null || x.SpecialCircumstances.IsApplied == false)
                     && (x.AllRenewalsEnabled == null || x.AllRenewalsEnabled.IsEnabled)
                     && x.PolicyStatus == PolicyStatus.Active
                     && !x.RenewalDates.OrderGenerated.HasValue
                     && x.RenewalDates.Initiated.HasValue
                     && x.RenewedPolicyId == null
                , cancellationToken);
        }
    }
}
