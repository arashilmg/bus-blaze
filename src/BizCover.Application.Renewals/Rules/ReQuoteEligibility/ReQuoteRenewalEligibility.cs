using BizCover.Application.Renewals.Services;

namespace BizCover.Application.Renewals.Rules.ReQuoteEligibility
{
    public interface IReQuoteRenewalEligibility
    {
        Task<RenewalEligibilityCheckResponse> CheckEligibility(Guid expiringPolicyId, string paymentFrequency, CancellationToken cancellationToken);
    }

    public class ReQuoteRenewalEligibility : IReQuoteRenewalEligibility
    {
        private readonly IRenewalService _renewalService;
        private readonly IReQuoteRenewalEligibilityService _reQuoteRenewalEligibilityService;

        public ReQuoteRenewalEligibility(
            IRenewalService renewalService,
            IReQuoteRenewalEligibilityService reQuoteRenewalEligibilityService)
        {
            _renewalService = renewalService;
            _reQuoteRenewalEligibilityService = reQuoteRenewalEligibilityService;
        }

        public async Task<RenewalEligibilityCheckResponse> CheckEligibility(Guid expiringPolicyId, string paymentFrequency, CancellationToken cancellationToken)
        {
            var renewalDetails = await _renewalService.GetRenewalDetailsForExpiringPolicy(expiringPolicyId, cancellationToken);

            var hasArrears = false;
            // Only check arrears for annual 
            if (paymentFrequency == Helpers.Constant.PaymentFrequencyMonthly)
            {
                hasArrears = await _renewalService.HasArrears(renewalDetails.ExpiringPolicyId, cancellationToken);
            }
            return _reQuoteRenewalEligibilityService.EnsureEligibility(
                new RenewalEligibilityCheckRequest
                {
                    Renewal = renewalDetails,
                    HasArrears = hasArrears
                });
        }
    }
}
