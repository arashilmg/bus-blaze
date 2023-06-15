using BizCover.Application.Renewals.DTO;
using BizCover.Application.Renewals.Services;
using BizCover.Application.Renewals.UseCases.RenewalDetails.Extensions;

namespace BizCover.Application.Renewals.UseCases.RenewalDetails
{
    public class GetRenewalDetails
    {
        private readonly IRenewalService _renewalService;

        public GetRenewalDetails(IRenewalService renewalService)
        {
            _renewalService = renewalService;
        }

        public async Task<GetRenewalDetailsResponse> Get(Guid orderId, CancellationToken cancellationToken)
        {
            var renewalDetails = await _renewalService.GetRenewalDetailsByOrderId(orderId, cancellationToken);

            return new GetRenewalDetailsResponse
            {
                ExpiringPolicyId = renewalDetails.ExpiringPolicyId,
                OrderId = renewalDetails.OrderId
            };
        }

        public async Task<RenewalDetailsDto> GetRenewalDetailsForExpiringPolicy(Guid expiringPolicyId, CancellationToken cancellationToken)
            => (await _renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, cancellationToken)).ToRenewalDetailsDto();

        public async Task<(RenewalDetailsDto? expiringPolicy, RenewalDetailsDto? renewedPolicy)> GetRenewalPolicyDetails(Guid policyId, CancellationToken cancellationToken)
        { 
            var policyDetails = (await _renewalService.GetRenewalDetailsForExpiringPolicy(policyId, cancellationToken))?.ToRenewalDetailsDto();
            var renewedPolicyDetails = (policyDetails.RenewedPolicyId.HasValue) 
                ? (await _renewalService.GetRenewalDetailsForExpiringPolicy(policyDetails.RenewedPolicyId.Value, cancellationToken)).ToRenewalDetailsDto()
                : (await _renewalService.GetRenewalDetailsForRenewedPolicy(policyId, cancellationToken))?.ToRenewalDetailsDto();
            
            return (policyDetails != null &&  (renewedPolicyDetails == null) || (renewedPolicyDetails != null && renewedPolicyDetails.RenewedPolicyId.HasValue)) ? (renewedPolicyDetails, policyDetails) : (policyDetails, renewedPolicyDetails);
        }

        public async Task UpdateAllRenewalsFlagToTrueFromNull(CancellationToken cancellationToken)
            => await _renewalService.UpdateAllRenewalsFlagToTrueFromNull(cancellationToken);
    }
}
