using BizCover.Application.Renewals.DTO;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases.RenewalDetails.Extensions;

namespace BizCover.Application.Renewals.UseCases.RenewalDetails
{
    public class UpdateRenewalDetails
    {
        private readonly IRenewalService _renewalService;

        public UpdateRenewalDetails(IRenewalService renewalService)
        {
            _renewalService = renewalService;
        }

        public async Task UpdateAllRenewalsFlagToTrueFromNull(CancellationToken cancellationToken)
            => await _renewalService.UpdateAllRenewalsFlagToTrueFromNull(cancellationToken);
    }
}
