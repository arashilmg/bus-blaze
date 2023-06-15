using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases
{
    public class UpdateEnableAllRenewalFlag
    {
        private readonly IRepository<Renewal> _renewalRepository;

        public UpdateEnableAllRenewalFlag(IRepository<Renewal> renewalRepository)
        {
            _renewalRepository = renewalRepository;
        }

        public async Task Update(Guid expiringPolicyId, bool enable, CancellationToken cancellationToken)
        {
            var renewal =
                (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken))
                .SingleOrDefault();

            if (renewal == null)
            {
                throw new NotFoundException<Renewal>(expiringPolicyId);
            }

            renewal.AllRenewalsEnabled ??= new RenewalsEnabled();
            renewal.AllRenewalsEnabled.IsEnabled = enable;
            renewal.AllRenewalsEnabled.UpdatedAt = DateTime.UtcNow;
            await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
        }
    }
}
