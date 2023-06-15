using BizCover.Application.Renewals.Helpers;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Renewals;

namespace BizCover.Application.Renewals.UseCases
{
    public class StartAutoRenewalOrderSubmission
    {
        private readonly IQueuePublisher _queuePublisher;
        private readonly IRepository<Renewal> _renewalRepository;
        private const int NumberOfDaysToTry = 6;

        public StartAutoRenewalOrderSubmission(IQueuePublisher queuePublisher,
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
                await _queuePublisher.Send(new SubmitAutoRenewalOrderCommand
                {
                    ExpiringPolicyId = renewal.ExpiringPolicyId,
                    OrderId = renewal.OrderId!.Value
                }, cancellationToken);
            }

        }

        private async Task<IEnumerable<Renewal>> GetRenewals(CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.AddDays(-NumberOfDaysToTry).Date;
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            return await _renewalRepository.FindAsync(
                x => x.RenewalDates!.OrderSubmission >= startDate && x.RenewalDates.OrderSubmission < endDate
                                                                  && x.OptIn
                                                                  && (x.SpecialCircumstances == null || x.SpecialCircumstances.IsApplied == false)
                                                                  && (x.SpecialCircumstances == null || x.AllRenewalsEnabled.IsEnabled)
                                                                  && x.OrderId.HasValue
                                                                  && x.RenewedPolicyId == null
                                                                  && x.RenewalDates.OrderGenerated.HasValue
                                                                  && x.PolicyStatus == PolicyStatus.Active,
                cancellationToken);
        }
    }
}
