using BizCover.Application.Renewals.Services;
using BizCover.Entity.Renewals;

namespace BizCover.Application.Renewals.Rules.AutoEligibility
{
    public interface IRenewalEligibility
    {
        Task<RenewalEligibilityResponse> CheckEligibility(Guid expiringPolicyId, string paymentFrequency, CancellationToken cancellationToken);
    }

    public class RenewalEligibility : IRenewalEligibility
    {
        private readonly IAutoRenewalEligibilityService _autoRenewalEligibilityService;
        private readonly IRenewalService _renewalService;

        public RenewalEligibility(IAutoRenewalEligibilityService autoRenewalEligibilityService, IRenewalService renewalService)
        {
            _autoRenewalEligibilityService = autoRenewalEligibilityService;
            _renewalService = renewalService;
        }

        public async Task<RenewalEligibilityResponse> CheckEligibility(Guid expiringPolicyId, string paymentFrequency, CancellationToken cancellationToken)
        {
            var renewalDetails = await _renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, cancellationToken);
            var hasArrears = false;
            // Only check arrears for monthly 
            if (paymentFrequency == Helpers.Constant.PaymentFrequencyMonthly)
            {
                hasArrears = await _renewalService.HasArrears(renewalDetails.ExpiringPolicyId, cancellationToken);
            }

            var response = _autoRenewalEligibilityService.EnsureEligibility(
                new RenewalEligibilityCheckRequest
                {
                    Renewal = renewalDetails,
                    HasArrears = hasArrears
                });

            return GetRenewalEligibilityResult(renewalDetails, response.IsEligible, response.Reason);
        }


        private static RenewalEligibilityResponse GetRenewalEligibilityResult(Renewal renewalDetails, bool isEligible, string message = "")
        {
            return new RenewalEligibilityResponse()
            {
                IsEligible = isEligible,
                HasAlreadyRenewed = renewalDetails.RenewedPolicyId.HasValue,
                AutoRenewalOptIn = renewalDetails.OptIn,
                Reason = message ?? string.Empty
            };
        }
    }

    public class RenewalEligibilityResponse
    {
        public bool IsEligible { get; set; }
        public bool HasAlreadyRenewed { get; set; }
        public bool AutoRenewalOptIn { get; set; }
        public string Reason { get; set; }
    }
}
