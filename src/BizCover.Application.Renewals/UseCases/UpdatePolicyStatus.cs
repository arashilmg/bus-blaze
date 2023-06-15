using BizCover.Entity.Renewals;
using BizCover.Framework.Application.Exceptions;
using BizCover.Framework.Application.Interfaces;

namespace BizCover.Application.Renewals.UseCases
{
    public class UpdatePolicyStatus
    {
        private readonly IRepository<Renewal> _renewalRepository;

        public UpdatePolicyStatus(IRepository<Renewal> renewalRepository)
        {
            _renewalRepository = renewalRepository;
        }

        public async Task UpdateStatus(Guid expiringPolicyId, string policyStatus, CancellationToken cancellationToken)
        {
            var renewal =
                (await _renewalRepository.FindAsync(x => x.ExpiringPolicyId == expiringPolicyId, cancellationToken))
                .SingleOrDefault();

            if (renewal == null)
            {
                throw new NotFoundException<Renewal>(expiringPolicyId);
            }

            renewal.PolicyStatus = policyStatus;
            await _renewalRepository.UpdateAsync(renewal.Id, renewal, cancellationToken);
        }
    }
}
