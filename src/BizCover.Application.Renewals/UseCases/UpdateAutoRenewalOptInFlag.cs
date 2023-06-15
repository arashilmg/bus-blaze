using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases
{
    public class UpdateAutoRenewalOptInFlag
    {
        private readonly IRepository<Renewal> _renewalRepository;
        private readonly IPublishAutoRenewalOptInFlagUpdatedEvent _publishAutoRenewalOptInFlagUpdatedEvent;

        public UpdateAutoRenewalOptInFlag(IRepository<Renewal> renewalRepository, IPublishAutoRenewalOptInFlagUpdatedEvent publishAutoRenewalOptInFlagUpdatedEvent)
        {
            _renewalRepository = renewalRepository;
            _publishAutoRenewalOptInFlagUpdatedEvent = publishAutoRenewalOptInFlagUpdatedEvent;
        }

        public async Task Update(Guid expiringPolicyId, bool optIn, CancellationToken cancellationToken)
        {
            var renewal =
                (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken))
                .SingleOrDefault();

            if (renewal == null)
            {
                throw new NotFoundException<Renewal>(expiringPolicyId);
            }
            
            renewal.OptIn = optIn;
            await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
            await _publishAutoRenewalOptInFlagUpdatedEvent.Publish(expiringPolicyId, cancellationToken);
        }
    }
}
