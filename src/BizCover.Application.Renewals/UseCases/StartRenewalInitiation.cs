using BizCover.Application.Renewals.Helpers;
using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Interfaces;
using BizCover.Messages.Renewals;
using Microsoft.Extensions.Logging;

namespace BizCover.Application.Renewals.UseCases
{
    public class StartRenewalInitiation
    {
        private readonly IRepository<Renewal> _renewalRepository;
        private readonly IQueuePublisher _queuePublisher;
        private readonly ILogger<StartRenewalInitiation> _logger;

        public StartRenewalInitiation(IQueuePublisher queuePublisher,
            IRepository<Renewal> renewalRepository,
            ILogger<StartRenewalInitiation> logger)
        {
            _renewalRepository = renewalRepository;
            _queuePublisher = queuePublisher;
            _logger = logger;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var renewals = await GetRenewals(cancellationToken);

            foreach (var renewal in renewals)
            {
                await _queuePublisher.Send(new InitiateRenewalCommand { ExpiringPolicyId = renewal.ExpiringPolicyId },
                    cancellationToken);
            }
        }

        private async Task<IEnumerable<Renewal>> GetRenewals(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetRenwals");
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(1);

            return await _renewalRepository.FindAsync(
                x => x.RenewalDates!.Initiation >= startDate 
                     && x.RenewalDates.Initiation < endDate
                     && x.PolicyStatus == PolicyStatus.Active
                     && !x.RenewalDates.Initiated.HasValue
                     && x.RenewedPolicyId == null,
            cancellationToken);
        }
    }
}
